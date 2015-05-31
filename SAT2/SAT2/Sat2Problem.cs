using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml;
using SAT2.Properties;

namespace SAT2
{
    public class Sat2Problem
    {
        #region Private Members
        private readonly Dictionary<string, Vertex> _vertices;
        #endregion Private Members
        #region Public Properties
        public Dictionary<string, Vertex> Vertices { get { return _vertices; } }
        #endregion Public Properties
        #region Constructors
        public Sat2Problem()
        {
            _vertices = new Dictionary<string, Vertex>();
        }
        #endregion Constructors
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
            if (!Vertices.TryGetValue(xName, out from))
            {
                from = new Vertex(xName);
                Vertices.Add(xName, from);
                Vertex negation;

                if (isNegative)
                    Vertices.Add("-" + xName, negation = new Vertex("-" + xName));
                else
                    Vertices.Add(xName.Substring(1), negation = new Vertex(xName.Substring(1)));
                from.Negation = negation;
                negation.Negation = from;
            }

            isNegative = y.InnerText.StartsWith("-");
            string yName = y.InnerText;
            if (!Vertices.TryGetValue(yName, out to))
            {
                to = new Vertex(yName);
                Vertices.Add(yName, to);
                Vertex negation;

                if (isNegative)
                    Vertices.Add(yName.Substring(1), negation = new Vertex(yName.Substring(1)));
                else
                    Vertices.Add("-" + yName, negation = new Vertex("-" + yName));
                to.Negation = negation;
                negation.Negation = to;
            }

            from.Neighbours.Add(to);
            to.Negation.Neighbours.Add(from.Negation);
        }
        private void CreateGraph(XmlNodeList formulas)
        {
            foreach (XmlNode formula in formulas)
            {
                if (formula.Attributes == null)
                    throw new Exception("No attributes");

                var x = formula.Attributes.GetNamedItem("x");
                var y = formula.Attributes.GetNamedItem("y");

                AddEdgeFromFormula(x, y);
            }
        }
        private bool FindValuations()
        {
            var vertex = FindFirstVertex();
            if (vertex == null) return false;
            vertex.Value.Value.Value = true;
            vertex.Value.Value.Negation.Value = false;

            if (!ValuateGraphFromVertex(vertex.Value.Value)) return false;

            foreach (var v in Vertices.Where(x => !x.Value.IsSet))
                if (!ValuateGraphFromVertex(v.Value)) return false;

            return true;
        }
        private bool ValuateGraphFromVertex(Vertex vertex)
        {
            if (vertex.IsSet && vertex.Value == false) return false;
            var verticesToEvaluate = new List<Vertex>();

            foreach (var neighbour in vertex.Neighbours)
            {
                if ((neighbour.IsSet && !neighbour.Value) || (neighbour.Negation.IsSet && neighbour.Negation.Value))
                    return false;

                if (!neighbour.IsSet)
                {
                    neighbour.Value = true;
                    neighbour.Negation.Value = false;
                    verticesToEvaluate.Add(neighbour);
                }
            }
            foreach (var neighbour in verticesToEvaluate)
                if (!ValuateGraphFromVertex(neighbour)) return false;
            return true;
        }
        /// <summary>
        /// Finds the first vertex that does not have a path to its negated value
        /// </summary>
        /// <returns>Found vertex or null</returns>
        private KeyValuePair<string, Vertex>? FindFirstVertex()
        {
            foreach (var v in Vertices)
                if (!CheckExistingPath(v.Value, v.Value.Negation))
                    return v;

            return null;
        }
        /// <summary>
        /// Checks whethe the path between two given vertices exists
        /// </summary>
        /// <param name="vertex">Start vertex</param>
        /// <param name="negation">Destination vertex</param>
        /// <returns>True if the path from the start to the destination exists, otherwise false</returns>
        private bool CheckExistingPath(Vertex vertex, Vertex negation)
        {
            if (vertex.Checked)
                return false;
            vertex.Checked = true;
            if (vertex == negation)
                return true;
            foreach (var neighbour in vertex.Neighbours)
                if (CheckExistingPath(neighbour, negation))
                    return true;

            return false;
        }
        private void CreateResultFile(string fileName)
        {
            XmlDocument xml = new XmlDocument();
            xml.AppendChild(xml.CreateElement(Resources.Sat2RootNodeName));
            var root = xml.SelectSingleNode(Resources.Sat2RootNodeName);

            foreach (var vertex in Vertices.Values.Where(x => !x.Name.StartsWith("-")))
            {
                var solution = xml.CreateElement(Resources.SolutionNode);
                solution.SetAttribute(Resources.VarNode, vertex.Name);
                solution.SetAttribute(Resources.ValueNode, (vertex.Value ? 1 : 0).ToString());
                root.AppendChild(solution);
            }

            xml.Save(fileName + Resources.SolutionFileNameSuffix);
        }
        #endregion Private Methods
        #region Public Methods
        /// <summary>
        /// Calculates the 2SAT problem from the specified file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void Run(string fileName)
        {
            try
            {
                var formulas = GetFormulasFromFile(fileName);
                CreateGraph(formulas);
            }
            catch (Exception e)
            {
                MessageBox.Show("Invalid file format.");
                return;
            }
            if (FindValuations())
            {
                CreateResultFile(fileName);
                MessageBox.Show("Done. Check file with results.");
            }
            else
                MessageBox.Show("No answers.");
        }
        #endregion Public Methods
    }
}

