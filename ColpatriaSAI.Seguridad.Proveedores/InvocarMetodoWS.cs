using System;
using DynamicProxyLibrary.DynamicProxy;

namespace ColpatriaSAI.Seguridad.Proveedores
{
    [Serializable]
    public class InvocarMetodoWS
    {
        private DynamicProxyFactory factory;
        private string serviceWsdlUri;

        public InvocarMetodoWS(string serviceWsdlUri)
    {
        this.serviceWsdlUri = serviceWsdlUri;
          
    }

      

        public string Invocarmetodo1( string wsdlportType, string metodo, params object[] parameters)

        {
            DynamicProxyFactory factory = new DynamicProxyFactory(this.serviceWsdlUri);
            DynamicProxy ObjetoProxy = factory.CreateProxy(wsdlportType);//"IAuthenticacion");
            

            string result = (string)ObjetoProxy.CallMethod(metodo, parameters);

            return result;

        }


        public object InvocarmetodoObjeto1(string wsdlportType, string metodo, params object[] parameters)
        {
            DynamicProxyFactory factory = new DynamicProxyFactory(this.serviceWsdlUri);
            DynamicProxy ObjetoProxy = factory.CreateProxy(wsdlportType);//"IAuthenticacion");
            object result = (object)ObjetoProxy.CallMethod(metodo, parameters);

           return result;
        }

        //ejemplos de como hacer los llamados al metodo del web service a traves del DynamicProxyFactory
        //private static void InvokeSimpleCalculator(DynamicProxyFactory factory)
        //{
        //    // create the DynamicProxy for the contract ISimpleCalculator and perform
        //    // operations
        //    Console.WriteLine("Creating DynamicProxy to SimpleCalculator Service");
        //    DynamicProxy simpleCalculatorProxy = factory.CreateProxy("ISimpleCalculator");
        //    // Call the Add service operation.
        //    double value1 = 100.00D;
        //    double value2 = 15.99D;
        //    double result = (double)simpleCalculatorProxy.CallMethod("Add", value1, value2);
        //    Console.WriteLine("Add({0},{1}) = {2}", value1, value2, result);

        //    // Call the Subtract service operation.
        //    value1 = 145.00D;
        //    value2 = 76.54D;
        //    result = (double)simpleCalculatorProxy.CallMethod("Subtract", value1, value2);
        //    Console.WriteLine("Subtract({0},{1}) = {2}", value1, value2, result);

        //    // Call the Multiply service operation.
        //    value1 = 9.00D;
        //    value2 = 81.25D;
        //    result = (double)simpleCalculatorProxy.CallMethod("Multiply", value1, value2);
        //    Console.WriteLine("Multiply({0},{1}) = {2}", value1, value2, result);

        //    // Call the Divide service operation.
        //    value1 = 22.00D;
        //    value2 = 7.00D;
        //    result = (double)simpleCalculatorProxy.CallMethod("Divide", value1, value2);
        //    Console.WriteLine("Divide({0},{1}) = {2}", value1, value2, result);

        //    // close the simpleCalculatorProxy
        //    simpleCalculatorProxy.Close();
        //}

        //private static void InvokeComplexCalculator(DynamicProxyFactory factory)
        //{
        //    // create the DynamicProxy for the contract IComplexCalculator and perform
        //    // operations
        //    Console.WriteLine("Creating DynamicProxy to ComplexCalculator Service");
        //    DynamicProxy complexCalculatorProxy = factory.CreateProxy("IComplexCalculator");

        //    // Call the Add service operation.
        //    DynamicComplexNumber value1 = new DynamicComplexNumber(factory.ProxyAssembly);
        //    value1.Real = 1; value1.Imaginary = 2;

        //    DynamicComplexNumber value2 = new DynamicComplexNumber(factory.ProxyAssembly);
        //    value2.Real = 3; value2.Imaginary = 4;

        //    object retval = complexCalculatorProxy.CallMethod("ComplexAdd",
        //        value1.ObjectInstance, value2.ObjectInstance);
        //    DynamicComplexNumber result = new DynamicComplexNumber(retval);

        //    Console.WriteLine("ComplexAdd({0} + {1}i, {2} + {3}i) = {4} + {5}i",
        //        value1.Real, value1.Imaginary, value2.Real, value2.Imaginary, result.Real, result.Imaginary);

        //    // Call the Subtract service operation.
        //    value1 = new DynamicComplexNumber(factory.ProxyAssembly);
        //    value1.Real = 1; value1.Imaginary = 2;

        //    value2 = new DynamicComplexNumber(factory.ProxyAssembly);
        //    value2.Real = 3; value2.Imaginary = 4;

        //    retval = complexCalculatorProxy.CallMethod("ComplexSubtract",
        //        value1.ObjectInstance, value2.ObjectInstance);
        //    result = new DynamicComplexNumber(retval);

        //    Console.WriteLine("ComplexSubtract({0} + {1}i, {2} + {3}i) = {4} + {5}i",
        //        value1.Real, value1.Imaginary, value2.Real, value2.Imaginary, result.Real, result.Imaginary);

        //    // Call the Multiply service operation.
        //    value1 = new DynamicComplexNumber(factory.ProxyAssembly);
        //    value1.Real = 2; value1.Imaginary = 3;

        //    value2 = new DynamicComplexNumber(factory.ProxyAssembly);
        //    value2.Real = 4; value2.Imaginary = 7;

        //    retval = complexCalculatorProxy.CallMethod("ComplexMultiply",
        //        value1.ObjectInstance, value2.ObjectInstance);
        //    result = new DynamicComplexNumber(retval);

        //    Console.WriteLine("ComplexMultiply({0} + {1}i, {2} + {3}i) = {4} + {5}i",
        //        value1.Real, value1.Imaginary, value2.Real, value2.Imaginary, result.Real, result.Imaginary);

        //    // Call the Divide service operation.
        //    value1 = new DynamicComplexNumber(factory.ProxyAssembly);
        //    value1.Real = 3; value1.Imaginary = 7;

        //    value2 = new DynamicComplexNumber(factory.ProxyAssembly);
        //    value2.Real = 5; value2.Imaginary = -2;

        //    retval = complexCalculatorProxy.CallMethod("ComplexDivide",
        //        value1.ObjectInstance, value2.ObjectInstance);
        //    result = new DynamicComplexNumber(retval);

        //    Console.WriteLine("ComplexDivide({0} + {1}i, {2} + {3}i) = {4} + {5}i",
        //        value1.Real, value1.Imaginary, value2.Real, value2.Imaginary, result.Real, result.Imaginary);

        //    complexCalculatorProxy.Close();
        //}

    }
}