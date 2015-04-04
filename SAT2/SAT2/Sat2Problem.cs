using System;
using System.Collections.Generic;
using System.Xml;
using SAT2.Properties;

namespace SAT2
{
    public class Sat2Problem
    {
        #region Private Members
        private Dictionary<string, Vertex> _vertices;
        private List<Edge> _edges;
        #endregion Private Members
        #region Public Properties
        #endregion Public Properties
        #region Private Methods
        private static XmlNodeList GetFormulasFromFile(string fileName)
        {
            var xml = new XmlDocument();
            xml.Load(fileName);
            if (xml.DocumentElement == null)
                throw new Exception("DocumentElement is null");

            var formulas = xml.DocumentElement.SelectNodes(Resources.Sat2ConditionNodeName);
            return formulas;
        }
        private void AddEdgeFromFormula(XmlNode x, XmlNode y)
        {
            Vertex from, to;
            bool isNegative = x.InnerText.StartsWith("-");
            string xName = isNegative ? x.InnerText.Substring(1) : "-" + x.InnerText;
            if (!_vertices.TryGetValue(xName, out from))
            {
                from = new Vertex(xName);
                _vertices.Add(xName, from);
                if (isNegative)
                    _vertices.Add("-" + xName, new Vertex("-" + xName));
                else
                    _vertices.Add(xName.Substring(1), new Vertex(xName.Substring(1)));
            }

            isNegative = y.InnerText.StartsWith("-");
            string yName = y.InnerText;
            if (!_vertices.TryGetValue(yName, out to))
            {
                to = new Vertex(yName);
                _vertices.Add(yName, to);
                if (isNegative)
                    _vertices.Add(yName.Substring(1), new Vertex(yName));
                else
                    _vertices.Add("-" + yName, new Vertex("-" + yName));
            }

            _edges.Add(new Edge(from, to));
        }
        #endregion Private Methods
        #region Public Methods
        /// <summary>
        /// Calculates the 2SAT problem from the specified file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void Run(string fileName)
        {
            var formulas = GetFormulasFromFile(fileName);
            _vertices = new Dictionary<string, Vertex>();
            _edges = new List<Edge>();

            foreach (XmlNode formula in formulas)
            {
                if (formula.Attributes == null)
                    throw new Exception("No attributes");

                var x = formula.Attributes.GetNamedItem("x");
                var y = formula.Attributes.GetNamedItem("y");

                AddEdgeFromFormula(x, y);
            }
        }
        #endregion Public Methods
        #region Commands
        #endregion Commands
    }
}

