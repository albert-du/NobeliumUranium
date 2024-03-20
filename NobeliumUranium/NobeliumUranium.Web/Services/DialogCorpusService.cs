using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using NobeliumUranium.Web.Models;


namespace NobeliumUranium.Web.Services
{
    public class DialogCorpusService
    {
        private string _dialogCorpusPath = Path.Combine(".", "DialogCorpus.json");
        public DialogCorpusService()
        {

        }
        private IEnumerable<DialogCorpus> _dialog;
        public IEnumerable<DialogCorpus> DialogCorpora 
        {
            get
            {
                if (_dialog == null)  _dialog = GetDialogCorpora();
                return _dialog;
            }
            set
            {
                using var outputStream = File.OpenWrite(_dialogCorpusPath);
                JsonSerializer.Serialize(
                    new Utf8JsonWriter(outputStream, new JsonWriterOptions
                    {
                        SkipValidation = true,
                        Indented = true
                    }),
                    value
                );
                _dialog = value;
            }
        }
        private IEnumerable<DialogCorpus> GetDialogCorpora()
        {
            // TODO:add file creation and sample data
            if (!File.Exists(_dialogCorpusPath))
            {
                using var stream = File.CreateText(_dialogCorpusPath);
                stream.WriteAsync(
                    @"[
  {
    ""Title"": ""Set the Corpus title here"",
    ""Description"": ""Create your own Dialog Corpora"",
    ""Conversations"": [
      [
        ""Hi"",
        ""Hello"",
        ""How are you doing?""
      ],
      [
        ""Hi what\u0027s your name"",
        ""I\u0027m Nobelium Uranium, What\u0027s your name""
      ]
    ]
  }
]");
            }

            using var jsonFileReader = File.OpenText(_dialogCorpusPath);
            return JsonSerializer.Deserialize<DialogCorpus[]>(jsonFileReader.ReadToEnd(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );
        }
    }
}
