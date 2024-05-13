using AddIn.Core.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Addin.Core.Tests.Helpers
{
  [TestClass]
  public class PathHelperTests
  {
    [TestMethod]
    public void TestRelativePathWithFilePath()
    {
      var solutionPath = @"D:\Dev\Programming 2022\ErrataNet6";
      var itemPath =     @"D:\Dev\Programming 2022\ErrataNet6\Errata.Text.Benchmarks\PartBenchmark.cs";
      var relativePath = PathHelper.RelativePath(solutionPath, itemPath);
      relativePath.Should().Be(@"Errata.Text.Benchmarks\PartBenchmark.cs");
    }

    [TestMethod]
    public void TestRelativePathWithDirectoryPath()
    {
      var solutionPath = @"D:\Dev\Programming 2022\ErrataNet6";
          var itemPath = @"D:\Dev\Programming 2022\ErrataNet6\Errata.Text.Benchmarks";
      var relativePath = PathHelper.RelativePath(solutionPath, itemPath);
      relativePath.Should().Be(@"Errata.Text.Benchmarks");
    }

    [TestMethod]
    public void TestRelativePathWithSameDirectory()
    {
      var solutionPath = @"D:\Dev\Programming 2022\ErrataNet6";
      var itemPath =     @"D:\Dev\Programming 2022\ErrataNet6";
      var relativePath = PathHelper.RelativePath(solutionPath, itemPath);
      relativePath.Should().Be(string.Empty);
    }

    [TestMethod]
    public void TestRelativePathWithNestedDirectory()
    {
      var solutionPath = @"D:\Dev\Programming 2022\ErrataNet6\Errata.Text.Benchmarks";
      var itemPath =     @"D:\Dev\Programming 2022\ErrataNet6\Errata.Text.Benchmarks\NestedDirectory";
      var relativePath = PathHelper.RelativePath(solutionPath, itemPath);
      relativePath.Should().Be(@"NestedDirectory");
    }
  }
}
