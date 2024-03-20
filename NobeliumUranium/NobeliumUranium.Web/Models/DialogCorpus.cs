using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NobeliumUranium.Web.Models
{
    public class DialogCorpus
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public string[][] Conversations { get; set; }

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}
