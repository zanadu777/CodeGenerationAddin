using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AddIn.Core.Hierarchy;
using AddIn.Core.Records;

namespace Addin.Core.Tests.Hierarchy
{
  [TestClass]
  public class ForestTest
  {
    [TestMethod]
    public void SearchResultTest()
    {
      var json = """
                 [{"Code":"    public static class XmlTests","File":"XmlTests.cs","Line":10,"Col":18,"Path":"D:\\Dev\\Programming 2022\\ErrataNet6\\Errata.Text\\XmlTests.cs","Extension":".cs","Project":"Errata.Text"},{"Code":"    public  class XmlSubstring:TextSubstring","File":"XmlSubstring.cs","Line":12,"Col":12,"Path":"D:\\Dev\\Programming 2022\\ErrataNet6\\Errata.Text\\XmlSubstring.cs","Extension":".cs","Project":"Errata.Text"},{"Code":"  public static  class XmlReaderExtensions","File":"XmlReaderExtensions.cs","Line":10,"Col":17,"Path":"D:\\Dev\\Programming 2022\\ErrataNet6\\Errata.Text\\XmlReaderExtensions.cs","Extension":".cs","Project":"Errata.Text"},{"Code":"    public  class TextSubstring","File":"TextSubstring.cs","Line":11,"Col":12,"Path":"D:\\Dev\\Programming 2022\\ErrataNet6\\Errata.Text\\TextSubstring.cs","Extension":".cs","Project":"Errata.Text"},{"Code":"    public class StringTransform","File":"StringTransform.cs","Line":9,"Col":11,"Path":"D:\\Dev\\Programming 2022\\ErrataNet6\\Errata.Text\\StringTransform.cs","Extension":".cs","Project":"Errata.Text"},{"Code":"  public static partial class StringExtensions","File":"StringExtensionsPart.cs","Line":9,"Col":24,"Path":"D:\\Dev\\Programming 2022\\ErrataNet6\\Errata.Text\\StringExtensionsPart.cs","Extension":".cs","Project":"Errata.Text"},{"Code":"    public static partial  class StringExtensions","File":"StringExtensions.cs","Line":7,"Col":27,"Path":"D:\\Dev\\Programming 2022\\ErrataNet6\\Errata.Text\\StringExtensions.cs","Extension":".cs","Project":"Errata.Text"},{"Code":"  public static class StreamExtensions","File":"StreamExtensions.cs","Line":10,"Col":16,"Path":"D:\\Dev\\Programming 2022\\ErrataNet6\\Errata.Text\\StreamExtensions.cs","Extension":".cs","Project":"Errata.Text"},{"Code":"    internal class ReadOnlySpanOfCharExtensions","File":"ReadOnlySpanOfCharExtensions.cs","Line":9,"Col":13,"Path":"D:\\Dev\\Programming 2022\\ErrataNet6\\Errata.Text\\ReadOnlySpanOfCharExtensions.cs","Extension":".cs","Project":"Errata.Text"},{"Code":" public   class Trim:StringTransform","File":"Trim.cs","Line":3,"Col":10,"Path":"D:\\Dev\\Programming 2022\\ErrataNet6\\Errata.Text\\Transforms\\Trim.cs","Extension":".cs","Project":"Errata.Text"},{"Code":"    public class Split : StringTransform","File":"Split.cs","Line":3,"Col":11,"Path":"D:\\Dev\\Programming 2022\\ErrataNet6\\Errata.Text\\Transforms\\Split.cs","Extension":".cs","Project":"Errata.Text"},{"Code":"    public class Sandwich:StringTransform","File":"Sandwich.cs","Line":9,"Col":11,"Path":"D:\\Dev\\Programming 2022\\ErrataNet6\\Errata.Text\\Transforms\\Sandwich.cs","Extension":".cs","Project":"Errata.Text"},{"Code":"    public  class Replace : StringTransform","File":"Replace.cs","Line":9,"Col":12,"Path":"D:\\Dev\\Programming 2022\\ErrataNet6\\Errata.Text\\Transforms\\Replace.cs","Extension":".cs","Project":"Errata.Text"},{"Code":" public   class RemoveEmpty : StringTransform","File":"RemoveEmpty.cs","Line":3,"Col":10,"Path":"D:\\Dev\\Programming 2022\\ErrataNet6\\Errata.Text\\Transforms\\RemoveEmpty.cs","Extension":".cs","Project":"Errata.Text"},{"Code":"    public class RegexMatch : StringTransform","File":"RegexMatch.cs","Line":5,"Col":11,"Path":"D:\\Dev\\Programming 2022\\ErrataNet6\\Errata.Text\\Transforms\\RegexMatch.cs","Extension":".cs","Project":"Errata.Text"},{"Code":" public   class LineExtractor:StringTransform","File":"LineExtractor.cs","Line":3,"Col":10,"Path":"D:\\Dev\\Programming 2022\\ErrataNet6\\Errata.Text\\Transforms\\LineExtractor.cs","Extension":".cs","Project":"Errata.Text"},{"Code":" public   class Join:StringTransform","File":"Join.cs","Line":3,"Col":10,"Path":"D:\\Dev\\Programming 2022\\ErrataNet6\\Errata.Text\\Transforms\\Join.cs","Extension":".cs","Project":"Errata.Text"},{"Code":"    public class XmlSubstring_Tests","File":"XmlSubstring_Tests.cs","Line":13,"Col":11,"Path":"D:\\Dev\\Programming 2022\\ErrataNet6\\Errata.Text.Tests\\XmlSubstring_Tests.cs","Extension":".cs","Project":"Errata.Text.Tests"},{"Code":"    public class TextSubstring_Tests","File":"TextSubstring_Tests.cs","Line":15,"Col":11,"Path":"D:\\Dev\\Programming 2022\\ErrataNet6\\Errata.Text.Tests\\TextSubstring_Tests.cs","Extension":".cs","Project":"Errata.Text.Tests"},{"Code":"    internal static class TestUtils","File":"TestUtils.cs","Line":10,"Col":20,"Path":"D:\\Dev\\Programming 2022\\ErrataNet6\\Errata.Text.Tests\\TestUtils.cs","Extension":".cs","Project":"Errata.Text.Tests"},{"Code":"  public class StringExtensions_Tests","File":"StringExtensions_Tests.cs","Line":11,"Col":9,"Path":"D:\\Dev\\Programming 2022\\ErrataNet6\\Errata.Text.Tests\\StringExtensions_Tests.cs","Extension":".cs","Project":"Errata.Text.Tests"},{"Code":"  public class PartBenchmark","File":"PartBenchmark.cs","Line":10,"Col":9,"Path":"D:\\Dev\\Programming 2022\\ErrataNet6\\Errata.Text.Benchmarks\\PartBenchmark.cs","Extension":".cs","Project":"Errata.Text.Benchmarks"},{"Code":"  public class Md5VsSha256","File":"Md5VsSha256.cs","Line":12,"Col":9,"Path":"D:\\Dev\\Programming 2022\\ErrataNet6\\Errata.Text.Benchmarks\\Md5VsSha256.cs","Extension":".cs","Project":"Errata.Text.Benchmarks"}]
                 """;
      List<SearchResult> searchResults = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SearchResult>>(json);

      var forest = new Forest<SearchResult>();
      forest.GroupBys.Add(new GroupBy<SearchResult> { GroupByMethod= x=> x.Project});
      forest.GroupBys.Add(new GroupBy<SearchResult> { GroupByMethod = x => x.File });

      forest.Add(searchResults);


    }
  }
}
