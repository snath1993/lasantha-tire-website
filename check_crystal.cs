using System;
using System.Reflection;

class Program {
    static void Main() {
        try {
            Type t = Type.GetTypeFromProgID("CrystalRuntime.Application");
            if (t != null) {
                Console.WriteLine("CrystalRuntime.Application FOUND");
            } else {
                Console.WriteLine("CrystalRuntime.Application NOT FOUND");
            }
            
            t = Type.GetTypeFromProgID("CrystalDesignRunTime.Application");
             if (t != null) {
                Console.WriteLine("CrystalDesignRunTime.Application FOUND");
            } else {
                Console.WriteLine("CrystalDesignRunTime.Application NOT FOUND");
            }
        } catch (Exception ex) {
            Console.WriteLine(ex.Message);
        }
    }
}
