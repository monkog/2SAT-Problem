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
            bool isNegative = x.InnerText.StartsWith("-") ? true : false;
            string xName = isNegative ? x.InnerText.Substring(1) : "-" + x.InnerText;
            if (!_vertices.TryGetValue(xName, out from))
            {
                _vertices.Add(xName, new Vertex());
                if (isNegative)
                    _vertices.Add("-" + xName, new Vertex());
                else
                    _vertices.Add(xName.Substring(1), new Vertex());
            }

            isNegative = y.InnerText.StartsWith("-") ? true : false;
            string yName = y.InnerText;
            if (!_vertices.TryGetValue(yName, out to))
            {
                _vertices.Add(yName, new Vertex());
                if (isNegative)
                    _vertices.Add(yName.Substring(1), new Vertex());
                else
                    _vertices.Add("-" + yName, new Vertex());
            }
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

