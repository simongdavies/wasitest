using System;
using System.IO;
using Wasmtime;
using Xunit;

namespace wasitest
{
    public class TestStdOut
    {
        [Fact]
        public void WriteToReadFromStdOut()
        {
          var filePath=System.IO.Path.GetTempFileName();
          Console.WriteLine(filePath);
          using var engine = new Engine();
          using var module = Module.FromFile(engine, @"Modules/optimized.wasm");
          using var host = new Host(engine);
          var config = new WasiConfiguration();

          config.WithStandardOutput(filePath);            
          host.DefineWasi("wasi_snapshot_preview1", config);

          using dynamic instance = host.Instantiate(module);
          instance._start();
          var line = File.ReadAllText(filePath);
          Assert.Equal("Hello, world!\n", line);
          Console.WriteLine(line);
          File.Delete(filePath);
        }
    }
}
