using System;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;

public class Sample {

  private const String filename = "test.xml";
  private const String stylesheet = "test.xsl";

  public static void Main() {               
    XslCompiledTransform xslt = new XslCompiledTransform();
    xslt.Load(stylesheet);
    XPathDocument doc = new XPathDocument(filename);          
    XmlWriter writer = XmlWriter.Create(Console.Out, xslt.OutputSettings);
    xslt.Transform(doc, writer);
    writer.Close();
  }
}