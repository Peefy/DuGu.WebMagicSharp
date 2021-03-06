﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WebMagicSharp.Selector
{
    /// <summary>
    /// Html node.
    /// </summary>
    public class HtmlNode : AbstractSelectable
    {

        List<DuGu.Standard.Html.HtmlNode> _elements;

        public HtmlNode(List<DuGu.Standard.Html.HtmlNode> elements)
        {
            _elements = elements;
        }

        public HtmlNode()
        {
            _elements = null;
        }

        public virtual List<DuGu.Standard.Html.HtmlNode> GetElements()
        {
            return _elements;
        }

        public override ISelectable JsonPath(string jsonPath)
        {
            throw new NotImplementedException();
        }

        public override ISelectable Links()
        {
            return SelectElements(new LinksSelector());
        }

        public override List<ISelectable> Nodes()
        {
            var selectables = new List<ISelectable>();
            foreach(var element in GetElements())
            {
                var childElements = new List<DuGu.Standard.Html.HtmlNode>(1)
                {
                    element
                };
                selectables.Add(new HtmlNode(childElements));
            }
            return selectables;
        }

        public override ISelectable SmartContent()
        {
            var smartContentSelector = Selectors.SmartContent();
            return Select(smartContentSelector, SourceTexts);
        }

        public override ISelectable Xpath(string xpath)
        {
            var xpathSelector = Selectors.XPath(xpath);
            return SelectElements(xpathSelector);
        }

        protected override List<string> SourceTexts
        {
            get
            {
                var sourceTexts = new List<string>();
                foreach (var element in GetElements())
                {
                    sourceTexts.Add(element.WriteContentTo());
                }
                return sourceTexts;
            }
        }

        public override ISelectable Select(ISelector selector)
        {
            return SelectList(selector);
        }

        public override ISelectable SelectList(ISelector selector)
        {
            if (selector is BaseElementSelector) {
                return SelectElements((BaseElementSelector)selector);
            }
            return SelectList(selector, SourceTexts);
        }

        protected ISelectable SelectElements(BaseElementSelector elementSelector)
        {
            var elementsTemp = GetElements();
            if (!elementSelector.HasAttribute())
            {
                List<DuGu.Standard.Html.HtmlNode> resultElements 
                    = new List<DuGu.Standard.Html.HtmlNode>();
                foreach(var element in elementsTemp)
                {
                    //var nodes = checkElementAndConvert(element);
                    var selectElementsTemp = 
                        elementSelector.SelectElements(element.WriteContentTo());
                    resultElements.AddRange(selectElementsTemp);
                }
                return new HtmlNode(resultElements);
            }
            else
            {
                // has attribute, consider as plaintext
                var resultStrings = new List<string>();
                var document = CheckElementAndConvert(elementsTemp);
                foreach (var element in elementsTemp)
                {                  
                    var selectList = elementSelector.SelectList(document);
                    resultStrings.AddRange(selectList);
                }
                return new PlainText(resultStrings);

            }
        }

        private DuGu.Standard.Html.HtmlDocument CheckElementAndConvert(List<DuGu.Standard.Html.HtmlNode> nodes)
        {
            var document = new DuGu.Standard.Html.HtmlDocument();
            foreach(var node in nodes)
            {
                document.DocumentNode.AppendChild(node);
            }
            return document;
        }

        public override ISelectable Jquery(string selector)
        {
            throw new NotImplementedException();
        }

        public override ISelectable Jquery(string selector, string attrName)
        {
            throw new NotImplementedException();
        }
    }
}
