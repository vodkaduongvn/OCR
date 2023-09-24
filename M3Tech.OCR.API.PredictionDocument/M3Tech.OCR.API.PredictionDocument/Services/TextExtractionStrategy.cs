using iTextSharp.text.pdf.parser;
using M3Tech.OCR.API.PredictionDocument.Models;
using System.Reflection.Metadata;
using System.Text;

namespace M3Tech.OCR.API.PredictionDocument.Services
{
    public class TextExtractionStrategy : LocationTextExtractionStrategy
    {
        private readonly ITextChunkLocationStrategy _tclStrategy;

        private readonly List<ContentLocation> _textLocations = new();
        private readonly List<ContentLocation> _imageLocations = new();
        private ContentLocation _textContentRendering = new();

        public TextExtractionStrategy() : this(new TextChunkLocationStrategyDefaultImp())
        {
        }

        /**
         * Creates a new text extraction renderer, with a custom strategy for
         * creating new TextChunkLocation objects based on the input of the
         * TextRenderInfo.
         * @param strat the custom strategy
         */
        public TextExtractionStrategy(ITextChunkLocationStrategy strat)
        {
            _tclStrategy = strat;
        }

        /**
         * Filters the provided list with the provided filter
         * @param textChunks a list of all TextChunks that this strategy found during processing
         * @param filter the filter to apply.  If null, filtering will be skipped.
         * @return the filtered list
         * @since 5.3.3
         */

        public override void RenderText(TextRenderInfo renderInfo)
        {
            LineSegment segment = renderInfo.GetBaseline();
            var startPoint = segment.GetStartPoint();

            var x = iTextSharp.text.Utilities.PointsToMillimeters(startPoint[0]);
            var y = iTextSharp.text.Utilities.PointsToMillimeters(startPoint[1]);

            // plus string in order to concat all characters in one line
            _textContentRendering.Content += renderInfo.GetText();
            _textContentRendering.Y = y;
            _textContentRendering.X = x;
        }

        public override void RenderImage(ImageRenderInfo renderInfo)
        {
            var startPoint = renderInfo.GetStartPoint();
            var x = iTextSharp.text.Utilities.PointsToMillimeters(startPoint[0]);
            var y = iTextSharp.text.Utilities.PointsToMillimeters(startPoint[1]);

            var imageObject = renderInfo.GetImage();

            _imageLocations.Add(new ContentLocation
            {
                X = x,
                Y = y,
                Content = Convert.ToBase64String(imageObject.GetImageAsBytes())
            });
        }

        public override void EndTextBlock()
        {
            try
            {
                _textContentRendering.Content += " ";

                if (_textLocations.Count > 0)
                {
                    var lastItem = _textLocations.Last();

                    if (lastItem.Y == _textContentRendering.Y) // the same Y, mean text on current line
                    {
                        var existingContent = string.Join("", _textLocations.Where(x => x.Y != _textContentRendering.Y).Select(x => x.Content));
                        if (existingContent.Length < _textContentRendering.Content.Length)
                        {
                            lastItem.Content = _textContentRendering.Content[existingContent.Length..];
                        }
                        else
                        {
                            _textLocations.Clear();
                        }
                    }
                    else // different Y, mean text on a new line
                    {
                        var existingContent = string.Join("", _textLocations.Select(x => x.Content));
                        if (existingContent.Length < _textContentRendering.Content.Length)
                        {
                            _textLocations.Add(new ContentLocation
                            {
                                X = _textContentRendering.X,
                                Y = _textContentRendering.Y,
                                Content = _textContentRendering.Content[existingContent.Length..]
                            });
                        }
                        else
                        {
                            _textLocations.Clear();
                        }
                    }
                }
                else
                {
                    _textLocations.Add(new ContentLocation
                    {
                        X = _textContentRendering.X,
                        Y = _textContentRendering.Y,
                        Content = _textContentRendering.Content
                    });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ContentLocation> GetTextLocations() => _textLocations;

        public List<ContentLocation> GetImageLocations() => _imageLocations;

    }
}
