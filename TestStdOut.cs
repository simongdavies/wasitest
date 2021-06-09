using System;
using System.IO;
using System.Threading.Tasks;
using Wasmtime;
using Xunit;

namespace wasitest
{
    public class TestStdOut
    {
        [Fact]
        public async Task WriteToReadFromStdOut()
        {
          using var file=new TempFile();
          Console.WriteLine(file.Path);
          using var engine = new Engine();
          using var module = Module.FromFile(engine, @"Modules/optimized.wasm");
          using var host = new Host(engine);
          var config = new WasiConfiguration();

          config.WithStandardOutput(file.Path);            
          host.DefineWasi("wasi_snapshot_preview1", config);

          using dynamic instance = host.Instantiate(module);
          instance._start();
          var fileStream = new FileStream(file.Path,FileMode.Open,  FileAccess.Read, FileShare.ReadWrite);
          using var reader = new StreamReader(fileStream);
          var line = await reader.ReadLineAsync();
          Assert.Equal("Hello, world!", line);
          Console.WriteLine(line);
        }
    }
}
