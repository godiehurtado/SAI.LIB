using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using ColpatriaSAI.Negocio.Componentes;

namespace ColpatriaSAI.Seguridad.Encripcion
{
    public class EncriptarDecriptar
    {
        #region Private members
        // si algoitmo Hashing no se especifica use  SHA-1.
        private static string DEFAULT_HASH_ALGORITHM = "SHA1";

        //Si el tamaño de clave no se especifica, utilice el más largo de 256 bits.
        private static int DEFAULT_KEY_SIZE = 256;

        // No permitir que el salt sea más de 255 bytes, ya que sólo tenemos
         // 1 byte para almacenar su longitud.

        private static int MAX_ALLOWED_SALT_LEN = 255;

      // No permitir que el salt sea menor de 4 bytes, ya que el uso de los primeros
         // 4 bytes de salt se usan  para almacenar su longitud.

        private static int MIN_ALLOWED_SALT_LEN = 4;

        // Valor salt aleatorio será de entre 4 y 8 bytes de longitud.

        private static int DEFAULT_MIN_SALT_LEN = MIN_ALLOWED_SALT_LEN;
        private static int DEFAULT_MAX_SALT_LEN = 8;

        // Utilizar estos miembros para salvar min y la longitud máxima de sal.
        private int minSaltLen = -1;
        private int maxSaltLen = -1;

        // Estos miembros serán utilizados para realizar el cifrado y descifrado.

        private ICryptoTransform encryptor = null;
        private ICryptoTransform decryptor = null;
        #endregion
        #region Constructors
        ///<summary>
         ///Utilice este constructor si usted está planeado para realizar el cifrado /
         ///Descifrado con clave de 256 bits, que se deriven con una iteración en la contraseña,
         ///Hashing sin salt, sin vector de inicialización, libro de códigos electrónicos
         ///(BCE), el modo, SHA-1 algoritmo de hash, y de 4 a 8 bytes de salt a lo largo.
         ///</Summary>
         ///<param Name="passPhrase">
        ///Palabra de paso derivada de  pseudo-aleatoria contraseña 
         ///La contraseña derivada se utilizan para generar la clave de cifrado.
        ///passPhrase puede ser cualquier cadena. En este ejemplo se supone que el
        ///Contraseña es una cadena ASCII.  la passPhrase de valor hay que tenerla en
         ///Secreto.
         ///</Param>
         ///<remarks>
         ///Este constructor no es recomendable debido a que no utiliza
         // Vector / inicialización y utiliza el modo de cifrado del BCE, que es menos
         ///Seguro que el modo CBC.
         ///</Remarks>

        public EncriptarDecriptar(string passPhrase) :
            this(passPhrase, null)
        {
        }

        /// <summary>
        ///Utilice este constructor si usted está planeado para realizar el cifrado /
        /// descifrado con clave de 256 bits, que se deriven con una iteración de una contraseña,
        /// hashing sin salt, encadenamiento de bloques de cifrado (CBC), SHA-1
        /// hash algoritmo, y de 4 a 8 bytes de salt de largo.
        /// </summary>
        /// <param name="passPhrase">
        /// Passphrase de la cual un pseudo password sera derivado
        /// El password derivado sera usado para generarl el encryption key
        /// Passphrase puede ser cualquier string. En este ejemplo se asumne que 
      /// Passphrase debera mantenerse en secreto
       /// </param>
        /// <param name="initVector">
        /// Vector de Inicializacion (IV).Este valor es requerido para encriptar el
        /// primner bloque del texto. Para RijndaelManaged class IV debera ser 
        /// Exactamente de  16 ASCII caracterses de  largo. los valores del IV no se mantendran en secreto
        /// </param>
        public EncriptarDecriptar(string passPhrase,
                                string initVector) :
            this(passPhrase, initVector, -1)
        {
        }

        /// <summary>
        /// Utilice este constructor si usted está planeado para realizar el cifrado/
        /// descifrado con clave de 256 bits, que se deriven con una iteración de una contraseña,
        /// hashing sin salt, encadenamiento de bloques de cifrado (CBC), SHA-1
        ///  hash algoritmo, y de 0 a 8 bytes de salt de largo.
        /// </summary>
        /// <param name="passPhrase">
        /// Passphrase de la cual un pseudo password sera derivado
        /// El password derivado sera usado para generarl el encryption key
        /// Passphrase puede ser cualquier string. En este ejemplo se asumne que 
        /// passphrasees un string ASSCII . Passphrase debera mantenerse en secreto
        /// </param>
        /// <param name="initVector">
        /// Vector de Inicializacion (IV).Este valor es requerido para encriptar el
        /// primner bloque del texto. Para RijndaelManaged class IV debera ser 
        /// Exactamente de  16 ASCII caracterses de  largo. los valores del IV no se mantendran en secreto
        /// </param>
        /// <param name="minSaltLen">
        /// Minimo tamaño (en  bytes) salt generado secuencialmente el cual debera ser adicionado en
        /// el principo del texto antes de que la encryption se ejecute.cuando este valor
        ///  es menos de 4 ,el minimo valor default debera ser usado(actualmente 4
        /// bytes).
        /// </param>
        public EncriptarDecriptar(string passPhrase,
                                string initVector,
                                int minSaltLen) :
            this(passPhrase, initVector, minSaltLen, -1)
        {
        }

        /// <summary>
        ///  Utilice este constructor si usted está planeado para realizar el cifrado/
        /// descifrado con clave de 256 bits, que se deriven con una iteración de una contraseña,
        /// hashing sin salt, encadenamiento de bloques de cifrado (CBC), SHA-1
        /// hashing algorithm. Use el minSaltLen and maxSaltLen parametros para
        /// especificar el tamaño  de el salt generado secuancialmente.
        /// </summary>
        /// <param name="passPhrase">
        /// Passphrase del cual el  pseudo-random password debera ser derivado.
        /// El password derivado debera ser usado para generar el encryption key.
        /// Passphrase puede ser cualqueir string. In this example we assume that the
        /// Passphrase puede ser cualquier string. En este ejemplo se asumne que 
        /// Passphrase debera mantenerse en secreto
        /// </param>
        /// <param name="initVector">
        /// Vector de Inicializacion (IV).Este valor es requerido para encriptar el
        /// primner bloque del texto. Para RijndaelManaged class IV debera ser 
        /// Exactamente de  16 ASCII caracterses de  largo. los valores del IV no se mantendran en secreto
        /// </param>
        /// <param name="minSaltLen">
        /// Minimo tamaño (en  bytes) salt generado secuencialmente el cual debera ser adicionado en
        /// el principo del texto antes de que la encryption se ejecute.cuando este valor
        ///  es menos de 4 ,el minimo valor default debera ser usado(actualmente 4
        /// bytes).
        /// </param>
        /// <param name="maxSaltLen">
        /// Maximo Tamaño (en bytes) de salt generado aleatoriamente el cual se adiciona 
        /// al principio del texto antes que la encripcion se ejecute. Cunado este
        /// valor es negativo o mayor a 255, el valor maximo default debera ser
        /// usado (actualmente 8 bytes). Si el valor maximo es 0 (zero) o es mas pequeño
        /// que el valor minimo especificado (el cual puede ser ajustado al valor default),
        /// salt no debera se usado y el valor del texto debera ser encriptado como es.
        ///En este caso, saltno debera ser procesado tambien durante la decripcion.
        /// </param>
        public EncriptarDecriptar(string passPhrase,
                                string initVector,
                                int minSaltLen,
                                int maxSaltLen) :
            this(passPhrase, initVector, minSaltLen, maxSaltLen, -1)
        {
        }

        /// <summary>
        /// Utilice este constructor si usted está planeado para realizar el cifrado/
        /// decripcion usando el llave derivada de una iteraccion del password,
        /// hashing sin salt, modo cipher block chaining (CBC) , y
        /// algoritmo SHA-1 .
        /// </summary>
        /// <param name="passPhrase">
        /// Passphrase del cual el  pseudo-random password debera ser derivado.
        /// El password derivado debera ser usado para generar el encryption key.
        /// Passphrase puede ser cualqueir string. In this example we assume that the
        /// Passphrase puede ser cualquier string. En este ejemplo se asumne que 
        /// Passphrase debera mantenerse en secreto
        /// </param>
        /// <param name="initVector">
        /// Vector de Inicializacion (IV).Este valor es requerido para encriptar el
        /// primner bloque del texto. Para RijndaelManaged class IV debera ser 
        /// Exactamente de  16 ASCII caracterses de  largo. los valores del IV no se mantendran en secreto
        /// </param>
        /// <param name="minSaltLen">
        /// Minimo tamaño (en  bytes) salt generado secuencialmente el cual debera ser adicionado en
        /// el principo del texto antes de que la encryption se ejecute.cuando este valor
        ///  es menos de 4 ,el minimo valor default debera ser usado(actualmente 4
        /// bytes).
        /// </param>
        /// <param name="maxSaltLen">
        /// Maximo Tamaño (en bytes) de salt generado aleatoriamente el cual se adiciona 
        /// al principio del texto antes que la encripcion se ejecute. Cunado este
        /// valor es negativo o mayor a 255, el valor maximo default debera ser
        /// usado (actualmente 8 bytes). Si el valor maximo es 0 (zero) o es mas pequeño
        /// que el valor minimo especificado (el cual puede ser ajustado al valor default),
        /// salt no debera se usado y el valor del texto debera ser encriptado como es.
        ///En este caso, saltno debera ser procesado tambien durante la decripcion.
        /// </param>
        /// <param name="keySize">
        /// Tamaño de la llave simetrica(en bits): 128, 192, or 256.
        /// </param>
        public EncriptarDecriptar(string passPhrase,
                                string initVector,
                                int minSaltLen,
                                int maxSaltLen,
                                int keySize) :
            this(passPhrase, initVector, minSaltLen, maxSaltLen, keySize, null)
        {
        }

        /// <summary>
        /// Utilice este constructor si usted está planeado para realizar el cifrado/
        /// decripcion usando el llave derivada de una iteraccion del password, hashing 
        /// sin salt, y modo cipher block chaining (CBC).
        /// </summary>
        /// <param name="passPhrase">
        /// Passphrase del cual el  pseudo-random password debera ser derivado.
        /// El password derivado debera ser usado para generar el encryption key.
        /// Passphrase puede ser cualqueir string. In this example we assume that the
        /// Passphrase puede ser cualquier string. En este ejemplo se asumne que 
        /// Passphrase debera mantenerse en secreto
        /// </param>
        /// <param name="initVector">
        /// Vector de Inicializacion (IV).Este valor es requerido para encriptar el
        /// primner bloque del texto. Para RijndaelManaged class IV debera ser 
        /// Exactamente de  16 ASCII caracterses de  largo. los valores del IV no se mantendran en secreto
        /// </param>
        /// <param name="minSaltLen">
        /// Minimo tamaño (en  bytes) salt generado secuencialmente el cual debera ser adicionado en
        /// el principo del texto antes de que la encryption se ejecute.cuando este valor
        ///  es menos de 4 ,el minimo valor default debera ser usado(actualmente 4
        /// bytes).
        /// </param>
        /// <param name="maxSaltLen">
        /// Maximo Tamaño (en bytes) de salt generado aleatoriamente el cual se adiciona 
        /// al principio del texto antes que la encripcion se ejecute. Cunado este
        /// valor es negativo o mayor a 255, el valor maximo default debera ser
        /// usado (actualmente 8 bytes). Si el valor maximo es 0 (zero) o es mas pequeño
        /// que el valor minimo especificado (el cual puede ser ajustado al valor default),
        /// salt no debera se usado y el valor del texto debera ser encriptado como es.
        ///En este caso, saltno debera ser procesado tambien durante la decripcion.
        /// </param>
        /// <param name="keySize">
        /// Tamaño de la llave simetrica(en bits): 128, 192, or 256.
        /// </param>
        /// <param name="hashAlgorithm">
        /// algoritmo Hashing :  "MD5" or "SHA1". SHA1 es recomendado.
        /// </param>
        public EncriptarDecriptar(string passPhrase,
                                string initVector,
                                int minSaltLen,
                                int maxSaltLen,
                                int keySize,
                                string hashAlgorithm) :
            this(passPhrase, initVector, minSaltLen, maxSaltLen, keySize,
                hashAlgorithm, null)
        {
        }

        /// <summary>
        /// Utilice este constructor si usted está planeado para realizar el cifrado/
        /// decryption usando la lave derivada desde una  iteration en el password , y
        /// modo cipher block chaining (CBC) .
        /// </summary>
        /// <param name="passPhrase">
        /// Passphrase del cual el  pseudo-random password debera ser derivado.
        /// El password derivado debera ser usado para generar el encryption key.
        /// Passphrase puede ser cualqueir string. In this example we assume that the
        /// Passphrase puede ser cualquier string. En este ejemplo se asumne que 
        /// Passphrase debera mantenerse en secreto
        /// </param>
        /// <param name="initVector">
        /// Vector de Inicializacion (IV).Este valor es requerido para encriptar el
        /// primner bloque del texto. Para RijndaelManaged class IV debera ser 
        /// Exactamente de  16 ASCII caracterses de  largo. los valores del IV no se mantendran en secreto
        /// </param>
        /// <param name="minSaltLen">
        /// Minimo tamaño (en  bytes) salt generado secuencialmente el cual debera ser adicionado en
        /// el principo del texto antes de que la encryption se ejecute.cuando este valor
        ///  es menos de 4 ,el minimo valor default debera ser usado(actualmente 4
        /// bytes).
        /// </param>
        /// <param name="maxSaltLen">
        /// Maximo Tamaño (en bytes) de salt generado aleatoriamente el cual se adiciona 
        /// al principio del texto antes que la encripcion se ejecute. Cunado este
        /// valor es negativo o mayor a 255, el valor maximo default debera ser
        /// usado (actualmente 8 bytes). Si el valor maximo es 0 (zero) o es mas pequeño
        /// que el valor minimo especificado (el cual puede ser ajustado al valor default),
        /// salt no debera se usado y el valor del texto debera ser encriptado como es.
        ///En este caso, saltno debera ser procesado tambien durante la decripcion.
        /// </param>
        /// <param name="keySize">
        /// Tamaño de la llave simetrica(en bits): 128, 192, or 256.
        /// </param>
        /// <param name="hashAlgorithm">
        /// algoritmo Hashing :  "MD5" or "SHA1". SHA1 es recomendado.
        /// </param>
        /// <param name="saltValue">
        /// valor del Salt  usado para el  password hashing durante la generacion de la llave. Este 
        /// no es el mismo salt que usamos durante la encripcion. Este parametro
        /// puede sert cualquier string.
        /// </param>
        public EncriptarDecriptar(string passPhrase,
                                string initVector,
                                int minSaltLen,
                                int maxSaltLen,
                                int keySize,
                                string hashAlgorithm,
                                string saltValue) :
            this(passPhrase, initVector, minSaltLen, maxSaltLen, keySize,
                hashAlgorithm, saltValue, 1)
        {
        }

        /// <summary>
        //Utilice este constructor si usted está planeado para realizar el cifrado/
        /// decrypcion con la llave derivada  desde los parametros especificados explicitamente
       /// </summary>
        /// <param name="passPhrase">
        /// Passphrase del cual el  pseudo-random password debera ser derivado.
        /// El password derivado debera ser usado para generar el encryption key.
        /// Passphrase puede ser cualqueir string. In this example we assume that the
        /// Passphrase puede ser cualquier string. En este ejemplo se asumne que 
        /// Passphrase debera mantenerse en secreto
        /// </param>
        /// <param name="initVector">
        /// Vector de Inicializacion (IV).Este valor es requerido para encriptar el
        /// primner bloque del texto. Para RijndaelManaged class IV debera ser 
        /// Exactamente de  16 ASCII caracterses de  largo. los valores del IV no se mantendran en secreto
        /// </param>
        /// <param name="minSaltLen">
        /// Minimo tamaño (en  bytes) salt generado secuencialmente el cual debera ser adicionado en
        /// el principo del texto antes de que la encryption se ejecute.cuando este valor
        ///  es menos de 4 ,el minimo valor default debera ser usado(actualmente 4
        /// bytes).
        /// </param>
        /// <param name="maxSaltLen">
        /// Maximo Tamaño (en bytes) de salt generado aleatoriamente el cual se adiciona 
        /// al principio del texto antes que la encripcion se ejecute. Cunado este
        /// valor es negativo o mayor a 255, el valor maximo default debera ser
        /// usado (actualmente 8 bytes). Si el valor maximo es 0 (zero) o es mas pequeño
        /// que el valor minimo especificado (el cual puede ser ajustado al valor default),
        /// salt no debera se usado y el valor del texto debera ser encriptado como es.
        ///En este caso, saltno debera ser procesado tambien durante la decripcion.
        /// </param>
        /// <param name="keySize">
        /// Tamaño de la llave simetrica(en bits): 128, 192, or 256.
        /// </param>
        /// <param name="hashAlgorithm">
        /// algoritmo Hashing :  "MD5" or "SHA1". SHA1 es recomendado.
        /// </param>
        /// <param name="saltValue">
        /// valor del Salt  usado para el  password hashing durante la generacion de la llave. Este 
        /// no es el mismo salt que usamos durante la encripcion. Este parametro
        /// puede sert cualquier string.
        /// </param>
        /// <param name="passwordIterations">
        /// numer5o de iteraciones usadas para el password hash. mas iteraciones son
        /// consideradas mas seguras pero tomara mas tiempo.
        /// </param>
        public EncriptarDecriptar(string passPhrase,
                                string initVector,
                                int minSaltLen,
                                int maxSaltLen,
                                int keySize,
                                string hashAlgorithm,
                                string saltValue,
                                int passwordIterations)
        {
            // guardar tamaño min del salt ; colocar como default si valor invalido pasa.
            if (minSaltLen < MIN_ALLOWED_SALT_LEN)
                this.minSaltLen = DEFAULT_MIN_SALT_LEN;
            else
                this.minSaltLen = minSaltLen;

            // guardar tamaño max del salt; colocar como default si valor invalido pasa.
            if (maxSaltLen < 0 || maxSaltLen > MAX_ALLOWED_SALT_LEN)
                this.maxSaltLen = DEFAULT_MAX_SALT_LEN;
            else
                this.maxSaltLen = maxSaltLen;

            // asignar el tamaño de la llave criptografica.
            if (keySize <= 0)
                keySize = DEFAULT_KEY_SIZE;

            // asignar el nombre del algoritmo. Asegure que este en mayuscula y no usar 
            // guioes, ej cambie "sha-1" a "SHA1".
            if (hashAlgorithm == null)
                hashAlgorithm = DEFAULT_HASH_ALGORITHM;
            else
                hashAlgorithm = hashAlgorithm.ToUpper().Replace("-", "");

            //  vector de inicializacion convertido a un arreglo de byte.
            byte[] initVectorBytes = null;

            // Salt usado para el password hashing (para generar la llave o key, no durante la encripcion)
            //  convertido a un arreglo de byte.
            byte[] saltValueBytes = null;

            // devolver bytes del vecto de inicializacion.
            if (initVector == null)
                initVectorBytes = new byte[0];
            else
                initVectorBytes = Encoding.ASCII.GetBytes(initVector);

            // devolver bytes del salt (usado en hashing).
            if (saltValue == null)
                saltValueBytes = new byte[0];
            else
                saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            // Generar password, el cual debera ser usado en la llave derivada.
            PasswordDeriveBytes password = new PasswordDeriveBytes(
                                                       passPhrase,
                                                       saltValueBytes,
                                                       hashAlgorithm,
                                                       passwordIterations);

            //  key convertida a un arreglo de  byte array ajustado  al taño de bits a bytes.
            byte[] keyBytes = password.GetBytes(keySize / 8);

            // Initialize Rijndael key object.
            RijndaelManaged symmetricKey = new RijndaelManaged();

            // If we do not have initialization vector, we cannot use the CBC mode.
            // The only alternative is the ECB mode (which is not as good).
            if (initVectorBytes.Length == 0)
                symmetricKey.Mode = CipherMode.ECB;
            else
                symmetricKey.Mode = CipherMode.CBC;

            // Create encryptor and decryptor, which we will use for cryptographic
            // operations.
            encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
        }
        #endregion
        #region Encryption routines
        /// <summary>
        /// Encrypts a string value generating a base64-encoded string.
        /// </summary>
        /// <param name="plainText">
        /// Plain text string to be encrypted.
        /// </param>
        /// <returns>
        /// Cipher text formatted as a base64-encoded string.
        /// </returns>
        public string Encrypt(string plainText)
        {
            return Encrypt(Encoding.UTF8.GetBytes(plainText));
        }

        /// <summary>
        /// Encrypts a byte array generating a base64-encoded string.
        /// </summary>
        /// <param name="plainTextBytes">
        /// Plain text bytes to be encrypted.
        /// </param>
        /// <returns>
        /// Cipher text formatted as a base64-encoded string.
        /// </returns>
        public string Encrypt(byte[] plainTextBytes)
        {
            return Convert.ToBase64String(EncryptToBytes(plainTextBytes));
        }

        /// <summary>
        /// Encrypts a string value generating a byte array of cipher text.
        /// </summary>
        /// <param name="plainText">
        /// Plain text string to be encrypted.
        /// </param>
        /// <returns>
        /// Cipher text formatted as a byte array.
        /// </returns>
        public byte[] EncryptToBytes(string plainText)
        {
            return EncryptToBytes(Encoding.UTF8.GetBytes(plainText));
        }

        /// <summary>
        /// Encrypts a byte array generating a byte array of cipher text.
        /// </summary>
        /// <param name="plainTextBytes">
        /// Plain text bytes to be encrypted.
        /// </param>
        /// <returns>
        /// Cipher text formatted as a byte array.
        /// </returns>
        public byte[] EncryptToBytes(byte[] plainTextBytes)
        {
            // Add salt at the beginning of the plain text bytes (if needed).
            byte[] plainTextBytesWithSalt = AddSalt(plainTextBytes);

            // Encryption will be performed using memory stream.
            MemoryStream memoryStream = new MemoryStream();

            // Let's make cryptographic operations thread-safe.
            lock (this)
            {
                // To perform encryption, we must use the Write mode.
                CryptoStream cryptoStream = new CryptoStream(
                                                   memoryStream,
                                                   encryptor,
                                                    CryptoStreamMode.Write);

                // Start encrypting data.
                cryptoStream.Write(plainTextBytesWithSalt,
                                    0,
                                   plainTextBytesWithSalt.Length);

                // Finish the encryption operation.
                cryptoStream.FlushFinalBlock();

                // Move encrypted data from memory into a byte array.
                byte[] cipherTextBytes = memoryStream.ToArray();

                // Close memory streams.
                memoryStream.Close();
                cryptoStream.Close();

                // Return encrypted data.
                return cipherTextBytes;
            }
        }
        #endregion

        #region Decryption routines
        /// <summary>
        /// Decrypts a base64-encoded cipher text value generating a string result.
        /// </summary>
        /// <param name="cipherText">
        /// Base64-encoded cipher text string to be decrypted.
        /// </param>
        /// <returns>
        /// Decrypted string value.
        /// </returns>
        public string Decrypt(string cipherText)
        {
            return Decrypt(Convert.FromBase64String(cipherText));
        }

        /// <summary>
        /// Decrypts a byte array containing cipher text value and generates a
        /// string result.
        /// </summary>
        /// <param name="cipherTextBytes">
        /// Byte array containing encrypted data.
        /// </param>
        /// <returns>
        /// Decrypted string value.
        /// </returns>
        public string Decrypt(byte[] cipherTextBytes)
        {
            return Encoding.UTF8.GetString(DecryptToBytes(cipherTextBytes));
        }

        /// <summary>
        /// Decrypts a base64-encoded cipher text value and generates a byte array
        /// of plain text data.
        /// </summary>
        /// <param name="cipherText">
        /// Base64-encoded cipher text string to be decrypted.
        /// </param>
        /// <returns>
        /// Byte array containing decrypted value.
        /// </returns>
        public byte[] DecryptToBytes(string cipherText)
        {
            return DecryptToBytes(Convert.FromBase64String(cipherText));
        }

        /// <summary>
        /// Decrypts a base64-encoded cipher text value and generates a byte array
        /// of plain text data.
        /// </summary>
        /// <param name="cipherTextBytes">
        /// Byte array containing encrypted data.
        /// </param>
        /// <returns>
        /// Byte array containing decrypted value.
        /// </returns>
        public byte[] DecryptToBytes(byte[] cipherTextBytes)
        {
            byte[] decryptedBytes = null;
            byte[] plainTextBytes = null;
            int decryptedByteCount = 0;
            int saltLen = 0;

            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

            // Since we do not know how big decrypted value will be, use the same
            // size as cipher text. Cipher text is always longer than plain text
            // (in block cipher encryption), so we will just use the number of
            // decrypted data byte after we know how big it is.
            decryptedBytes = new byte[cipherTextBytes.Length];

            // Let's make cryptographic operations thread-safe.
            lock (this)
            {
                // To perform decryption, we must use the Read mode.
                CryptoStream cryptoStream = new CryptoStream(
                                                   memoryStream,
                                                   decryptor,
                                                   CryptoStreamMode.Read);

                // Decrypting data and get the count of plain text bytes.
                decryptedByteCount = cryptoStream.Read(decryptedBytes,
                                                        0,
                                                        decryptedBytes.Length);
                // Release memory.
                memoryStream.Close();
                cryptoStream.Close();
            }

            // If we are using salt, get its length from the first 4 bytes of plain
            // text data.
            if (maxSaltLen > 0 && maxSaltLen >= minSaltLen)
            {
                saltLen = (decryptedBytes[0] & 0x03) |
                            (decryptedBytes[1] & 0x0c) |
                            (decryptedBytes[2] & 0x30) |
                            (decryptedBytes[3] & 0xc0);
            }

            // Allocate the byte array to hold the original plain text (without salt).
            plainTextBytes = new byte[decryptedByteCount - saltLen];

            // Copy original plain text discarding the salt value if needed.
            Array.Copy(decryptedBytes, saltLen, plainTextBytes,
                        0, decryptedByteCount - saltLen);

            // Return original plain text value.
            return plainTextBytes;
        }
        #endregion

        #region Helper functions
        /// <summary>
        /// Adds an array of randomly generated bytes at the beginning of the
        /// array holding original plain text value.
        /// </summary>
        /// <param name="plainTextBytes">
        /// Byte array containing original plain text value.
        /// </param>
        /// <returns>
        /// Either original array of plain text bytes (if salt is not used) or a
        /// modified array containing a randomly generated salt added at the 
        /// beginning of the plain text bytes. 
        /// </returns>
        private byte[] AddSalt(byte[] plainTextBytes)
        {
            // The max salt value of 0 (zero) indicates that we should not use 
            // salt. Also do not use salt if the max salt value is smaller than
            // the min value.
            if (maxSaltLen == 0 || maxSaltLen < minSaltLen)
                return plainTextBytes;

            // Generate the salt.
            byte[] saltBytes = GenerateSalt();

            // Allocate array which will hold salt and plain text bytes.
            byte[] plainTextBytesWithSalt = new byte[plainTextBytes.Length +
                                                     saltBytes.Length];
            // First, copy salt bytes.
            Array.Copy(saltBytes, plainTextBytesWithSalt, saltBytes.Length);

            // Append plain text bytes to the salt value.
            Array.Copy(plainTextBytes, 0,
                        plainTextBytesWithSalt, saltBytes.Length,
                        plainTextBytes.Length);

            return plainTextBytesWithSalt;
        }

        /// <summary>
        /// Generates an array holding cryptographically strong bytes.
        /// </summary>
        /// <returns>
        /// Array of randomly generated bytes.
        /// </returns>
        /// <remarks>
        /// Salt size will be defined at random or exactly as specified by the
        /// minSlatLen and maxSaltLen parameters passed to the object constructor.
        /// The first four bytes of the salt array will contain the salt length
        /// split into four two-bit pieces.
        /// </remarks>
        private byte[] GenerateSalt()
        {
            // We don't have the length, yet.
            int saltLen = 0;

            // If min and max salt values are the same, it should not be random.
            if (minSaltLen == maxSaltLen)
                saltLen = minSaltLen;
            // Use random number generator to calculate salt length.
            else
                saltLen = GenerateRandomNumber(minSaltLen, maxSaltLen);

            // Allocate byte array to hold our salt.
            byte[] salt = new byte[saltLen];

            // Populate salt with cryptographically strong bytes.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            rng.GetNonZeroBytes(salt);

            // Split salt length (always one byte) into four two-bit pieces and
            // store these pieces in the first four bytes of the salt array.
            salt[0] = (byte)((salt[0] & 0xfc) | (saltLen & 0x03));
            salt[1] = (byte)((salt[1] & 0xf3) | (saltLen & 0x0c));
            salt[2] = (byte)((salt[2] & 0xcf) | (saltLen & 0x30));
            salt[3] = (byte)((salt[3] & 0x3f) | (saltLen & 0xc0));

            return salt;
        }

        /// <summary>
        /// Generates random integer.
        /// </summary>
        /// <param name="minValue">
        /// Min value (inclusive).
        /// </param>
        /// <param name="maxValue">
        /// Max value (inclusive).
        /// </param>
        /// <returns>
        /// Random integer value between the min and max values (inclusive).
        /// </returns>
        /// <remarks>
        /// This methods overcomes the limitations of .NET Framework's Random
        /// class, which - when initialized multiple times within a very short
        /// period of time - can generate the same "random" number.
        /// </remarks>
        private int GenerateRandomNumber(int minValue, int maxValue)
        {
            // We will make up an integer seed from 4 bytes of this array.
            byte[] randomBytes = new byte[4];

            // Generate 4 random bytes.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);

            // Convert four random bytes into a positive integer value.
            int seed = ((randomBytes[0] & 0x7f) << 24) |
                        (randomBytes[1] << 16) |
                        (randomBytes[2] << 8) |
                        (randomBytes[3]);

            // Now, this looks more like real randomization.
            Random random = new Random(seed);

            // Calculate a random number.
            return random.Next(minValue, maxValue + 1);
        }
        #endregion
    }

}

    ///// <summary>
    ///// Illustrates the use of RijndaelEnhanced class to encrypt and decrypt data
    ///// using a random salt value.
    ///// </summary>
    //public class RijndaelEnhancedTest
    //{
    //    /// <summary>
    //    /// The main entry point for the application.
    //    /// </summary>
    //    [STAThread]
    //    static void Main(string[] args)
    //    {
    //        string plainText = "Hello, World!";    // original plaintext
    //        string cipherText = "";                 // encrypted text
    //        string passPhrase = "Pas5pr@se";        // can be any string
    //        string initVector = "@1B2c3D4e5F6g7H8"; // must be 16 bytes

    //        // Before encrypting data, we will append plain text to a random
    //        // salt value, which will be between 4 and 8 bytes long (implicitly
    //        // used defaults).
    //        RijndaelEnhanced rijndaelKey =
    //            new RijndaelEnhanced(passPhrase, initVector);

    //        Console.WriteLine(String.Format("Plaintext   : {0}\n", plainText));

    //        // Encrypt the same plain text data 10 time (using the same key,
    //        // initialization vector, etc) and see the resulting cipher text;
    //        // encrypted values will be different.
    //        for (int i = 0; i < 10; i++)
    //        {
    //            cipherText = rijndaelKey.Encrypt(plainText);
    //            Console.WriteLine(
    //                String.Format("Encrypted #{0}: {1}", i, cipherText));
    //            plainText = rijndaelKey.Decrypt(cipherText);
    //        }

    //        // Make sure we got decryption working correctly.
    //        Console.WriteLine(String.Format("\nDecrypted   :{0}", plainText));
    //    }

    

