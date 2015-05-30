using System.Collections.Generic;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAT2;

namespace SAT2UnitTests
{
    [TestClass]
    public class UnitTests
    {
        private const string SatProblemPassing = @"<?xml version=""1.0"" encoding=""iso-8859-2"" ?><SAT2><Condition x=""a"" y=""b""/><Condition x=""b"" y=""-c""/><Condition x=""-a"" y=""-c""/><Condition x=""-b"" y=""d""/></SAT2>";
        private const string SatProblemPassing2 = @"<?xml version=""1.0"" encoding=""iso-8859-2"" ?><SAT2><Condition x=""0"" y=""2""/><Condition x=""0"" y=""-3""/><Condition x=""1"" y=""-3""/><Condition x=""1"" y=""-4""/><Condition x=""2"" y=""-4""/><Condition x=""0"" y=""-5""/><Condition x=""1"" y=""-5""/><Condition x=""2"" y=""-5""/><Condition x=""3"" y=""6""/><Condition x=""4"" y=""6""/><Condition x=""5"" y=""6""/></SAT2>";
        private const string SatProblemUnpassing = @"<?xml version=""1.0"" encoding=""iso-8859-2"" ?><SAT2><Condition x=""a"" y=""b""/><Condition x=""-a"" y=""b""/><Condition x=""-a"" y=""-b""/><Condition x=""a"" y=""-b""/></SAT2>";

        /// <summary>
        /// Tests adding vertices from formula.
        /// </summary>
        [TestMethod]
        public void TestAddVertexFromFormula()
        {
            var xml = new XmlDocument();
            xml.LoadXml(SatProblemPassing);

            var conditions = xml.DocumentElement.SelectNodes("Condition");

            Sat2Problem problem = new Sat2Problem();
            problem.AddEdgeFromFormula(conditions[0].Attributes.GetNamedItem("x"), conditions[0].Attributes.GetNamedItem("y"));

            Assert.IsTrue(problem.Vertices.ContainsKey("a"));
            Assert.IsTrue(problem.Vertices.ContainsKey("-a"));
            Assert.IsTrue(problem.Vertices.ContainsKey("b"));
            Assert.IsTrue(problem.Vertices.ContainsKey("-b"));
            Assert.IsTrue(problem.Vertices.Count == 4);

            problem.AddEdgeFromFormula(conditions[1].Attributes.GetNamedItem("x"), conditions[1].Attributes.GetNamedItem("y"));
            Assert.IsTrue(problem.Vertices.Count == 6);

            problem.AddEdgeFromFormula(conditions[2].Attributes.GetNamedItem("x"), conditions[2].Attributes.GetNamedItem("y"));
            Assert.IsTrue(problem.Vertices.Count == 6);

            problem.AddEdgeFromFormula(conditions[3].Attributes.GetNamedItem("x"), conditions[3].Attributes.GetNamedItem("y"));

            Assert.IsTrue(problem.Vertices.ContainsKey("c"));
            Assert.IsTrue(problem.Vertices.ContainsKey("-c"));
            Assert.IsTrue(problem.Vertices.ContainsKey("d"));
            Assert.IsTrue(problem.Vertices.ContainsKey("-d"));
            Assert.IsTrue(problem.Vertices.Count == 8);
        }
        /// <summary>
        /// Tests adding edges from formula.
        /// </summary>
        [TestMethod]
        public void TestAddEdgeFromFormula()
        {
            var xml = new XmlDocument();
            xml.LoadXml(SatProblemPassing);

            var conditions = xml.DocumentElement.SelectNodes("Condition");

            Sat2Problem problem = new Sat2Problem();
            problem.AddEdgeFromFormula(conditions[0].Attributes.GetNamedItem("x"), conditions[0].Attributes.GetNamedItem("y"));

            Assert.IsTrue(problem.Vertices["a"].Negation == problem.Vertices["-a"]);
            Assert.IsTrue(problem.Vertices["a"].Neighbours.Count == 0);
            Assert.IsTrue(problem.Vertices["-a"].Neighbours.Count == 1);
            Assert.IsTrue(problem.Vertices["-a"].Neighbours.Contains(problem.Vertices["b"]));

            Assert.IsTrue(problem.Vertices["b"].Negation == problem.Vertices["-b"]);
            Assert.IsTrue(problem.Vertices["b"].Neighbours.Count == 0);
            Assert.IsTrue(problem.Vertices["-b"].Neighbours.Count == 1);
            Assert.IsTrue(problem.Vertices["-b"].Neighbours.Contains(problem.Vertices["a"]));

            problem.AddEdgeFromFormula(conditions[1].Attributes.GetNamedItem("x"), conditions[1].Attributes.GetNamedItem("y"));
            Assert.IsTrue(problem.Vertices["c"].Negation == problem.Vertices["-c"]);
            Assert.IsTrue(problem.Vertices["c"].Neighbours.Count == 1);
            Assert.IsTrue(problem.Vertices["-c"].Neighbours.Count == 0);
            Assert.IsTrue(problem.Vertices["c"].Neighbours.Contains(problem.Vertices["b"]));
            Assert.IsTrue(problem.Vertices["-b"].Neighbours.Contains(problem.Vertices["-c"]));

            problem.AddEdgeFromFormula(conditions[2].Attributes.GetNamedItem("x"), conditions[2].Attributes.GetNamedItem("y"));
            Assert.IsTrue(problem.Vertices["a"].Neighbours.Count == 1);
            Assert.IsTrue(problem.Vertices["-a"].Neighbours.Count == 1);
            Assert.IsTrue(problem.Vertices["c"].Neighbours.Count == 2);
            Assert.IsTrue(problem.Vertices["-c"].Neighbours.Count == 0);
            Assert.IsTrue(problem.Vertices["c"].Neighbours.Contains(problem.Vertices["-a"]));
            Assert.IsTrue(problem.Vertices["a"].Neighbours.Contains(problem.Vertices["-c"]));

            problem.AddEdgeFromFormula(conditions[3].Attributes.GetNamedItem("x"), conditions[3].Attributes.GetNamedItem("y"));
            Assert.IsTrue(problem.Vertices["d"].Negation == problem.Vertices["-d"]);
            Assert.IsTrue(problem.Vertices["d"].Neighbours.Count == 0);
            Assert.IsTrue(problem.Vertices["-d"].Neighbours.Count == 1);
            Assert.IsTrue(problem.Vertices["b"].Neighbours.Count == 1);
            Assert.IsTrue(problem.Vertices["-b"].Neighbours.Count == 2);
            Assert.IsTrue(problem.Vertices["b"].Neighbours.Contains(problem.Vertices["d"]));
            Assert.IsTrue(problem.Vertices["-d"].Neighbours.Contains(problem.Vertices["-b"]));

            Assert.IsTrue(problem.Vertices["a"].Neighbours.Count == 1);
            Assert.IsTrue(problem.Vertices["-a"].Neighbours.Count == 1);
            Assert.IsTrue(problem.Vertices["b"].Neighbours.Count == 1);
            Assert.IsTrue(problem.Vertices["-b"].Neighbours.Count == 2);
            Assert.IsTrue(problem.Vertices["c"].Neighbours.Count == 2);
            Assert.IsTrue(problem.Vertices["-c"].Neighbours.Count == 0);
            Assert.IsTrue(problem.Vertices["d"].Neighbours.Count == 0);
            Assert.IsTrue(problem.Vertices["-d"].Neighbours.Count == 1);
        }
        /// <summary>
        /// Tests creating the graph.
        /// </summary>
        [TestMethod]
        public void TestCreateGraph()
        {
            var xml = new XmlDocument();
            xml.LoadXml(SatProblemPassing);

            var conditions = xml.DocumentElement.SelectNodes("Condition");

            Sat2Problem problem = new Sat2Problem();
            problem.CreateGraph(conditions);

            Assert.IsTrue(problem.Vertices.ContainsKey("a"));
            Assert.IsTrue(problem.Vertices.ContainsKey("-a"));
            Assert.IsTrue(problem.Vertices.ContainsKey("b"));
            Assert.IsTrue(problem.Vertices.ContainsKey("-b"));
            Assert.IsTrue(problem.Vertices.ContainsKey("c"));
            Assert.IsTrue(problem.Vertices.ContainsKey("-c"));
            Assert.IsTrue(problem.Vertices.ContainsKey("d"));
            Assert.IsTrue(problem.Vertices.ContainsKey("-d"));
            Assert.IsTrue(problem.Vertices.Count == 8);
        }
        /// <summary>
        /// Checks if the path from one vertex to another exists.
        /// </summary>
        [TestMethod]
        public void TestCheckExistingPath()
        {
            var xml = new XmlDocument();
            xml.LoadXml(SatProblemPassing);

            var conditions = xml.DocumentElement.SelectNodes("Condition");

            Sat2Problem problem = new Sat2Problem();
            problem.CreateGraph(conditions);
            var vertices = problem.Vertices;

            Assert.IsTrue(problem.CheckExistingPath(vertices["a"], vertices["a"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["a"], vertices["-a"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["a"], vertices["b"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["a"], vertices["-b"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["a"], vertices["c"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(problem.CheckExistingPath(vertices["a"], vertices["-c"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["a"], vertices["d"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["a"], vertices["-d"]));
            ResetVertices(problem.Vertices);

            Assert.IsTrue(!problem.CheckExistingPath(vertices["-a"], vertices["a"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(problem.CheckExistingPath(vertices["-a"], vertices["-a"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(problem.CheckExistingPath(vertices["-a"], vertices["b"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["-a"], vertices["-b"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["-a"], vertices["c"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["-a"], vertices["-c"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(problem.CheckExistingPath(vertices["-a"], vertices["d"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["-a"], vertices["-d"]));
            ResetVertices(problem.Vertices);

            Assert.IsTrue(!problem.CheckExistingPath(vertices["b"], vertices["a"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["b"], vertices["-a"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(problem.CheckExistingPath(vertices["b"], vertices["b"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["b"], vertices["-b"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["b"], vertices["c"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["b"], vertices["-c"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(problem.CheckExistingPath(vertices["b"], vertices["d"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["b"], vertices["-d"]));
            ResetVertices(problem.Vertices);

            Assert.IsTrue(!problem.CheckExistingPath(vertices["c"], vertices["a"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(problem.CheckExistingPath(vertices["c"], vertices["-a"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(problem.CheckExistingPath(vertices["c"], vertices["b"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["c"], vertices["-b"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(problem.CheckExistingPath(vertices["c"], vertices["c"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["c"], vertices["-c"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(problem.CheckExistingPath(vertices["c"], vertices["d"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["c"], vertices["-d"]));
            ResetVertices(problem.Vertices);

            Assert.IsTrue(!problem.CheckExistingPath(vertices["-c"], vertices["a"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["-c"], vertices["-a"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["-c"], vertices["b"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["-c"], vertices["-b"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["-c"], vertices["c"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(problem.CheckExistingPath(vertices["-c"], vertices["-c"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["-c"], vertices["d"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["-c"], vertices["-d"]));
            ResetVertices(problem.Vertices);

            Assert.IsTrue(!problem.CheckExistingPath(vertices["d"], vertices["a"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["d"], vertices["-a"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["d"], vertices["b"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["d"], vertices["-b"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["d"], vertices["c"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["d"], vertices["-c"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(problem.CheckExistingPath(vertices["d"], vertices["d"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["d"], vertices["-d"]));
            ResetVertices(problem.Vertices);

            Assert.IsTrue(problem.CheckExistingPath(vertices["-d"], vertices["a"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["-d"], vertices["-a"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["-d"], vertices["b"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(problem.CheckExistingPath(vertices["-d"], vertices["-b"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["-d"], vertices["c"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(problem.CheckExistingPath(vertices["-d"], vertices["-c"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(!problem.CheckExistingPath(vertices["-d"], vertices["d"]));
            ResetVertices(problem.Vertices);
            Assert.IsTrue(problem.CheckExistingPath(vertices["-d"], vertices["-d"]));
        }
        private void ResetVertices(Dictionary<string, Vertex> vertices)
        {
            foreach (var vertex in vertices.Values)
                vertex.Checked = false;
        }
        /// <summary>
        /// Finds the valuations.
        /// </summary>
        [TestMethod]
        public void TestFindValuations()
        {
            var xml = new XmlDocument();
            xml.LoadXml(SatProblemPassing);
            var conditions = xml.DocumentElement.SelectNodes("Condition");
            Sat2Problem problem = new Sat2Problem();
            problem.CreateGraph(conditions);
            Assert.IsTrue(problem.FindValuations());

            var xmlPassing2 = new XmlDocument();
            xmlPassing2.LoadXml(SatProblemPassing2);
            var conditionsPassing2 = xmlPassing2.DocumentElement.SelectNodes("Condition");
            Sat2Problem problemPassing2 = new Sat2Problem();
            problemPassing2.CreateGraph(conditionsPassing2);
            Assert.IsTrue(problemPassing2.FindValuations());

            var xmlUnpassed = new XmlDocument();
            xmlUnpassed.LoadXml(SatProblemUnpassing);
            var conditionsUnpassed = xmlUnpassed.DocumentElement.SelectNodes("Condition");
            Sat2Problem problemUnpassed = new Sat2Problem();
            problemUnpassed.CreateGraph(conditionsUnpassed);
            Assert.IsFalse(problemUnpassed.FindValuations());
        }
    }
}
