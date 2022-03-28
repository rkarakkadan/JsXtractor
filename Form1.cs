using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace JsXtractor
{
    public partial class Form1 : Form
    {
        private IEnumerable<KeyValuePair<string, JValue>> _fields;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length != 0 && textBox2.Text.Length != 0)
            {
                try
                {
                 textBox3.Text = string.Join(",\r\n", _fields.Where(x => x.Key.EndsWith(textBox2.Text)).Select(x => x.Value).ToList());
                }
                catch (Exception ex) { }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_Validated(object sender, EventArgs e)
        {
            if (textBox1.Text.Length != 0)
            {
                try
                {
                    string sourceJson = textBox1.Text;
                    var json = JToken.Parse(sourceJson);
                    var fieldsCollector = new JsonFieldsCollector(json);
                    _fields = fieldsCollector.GetAllFields();
                    comboBox1.DataSource = _fields.ToList();
                }
                catch (Exception ex) {
                    MessageBox.Show("Please check the json format!");
                }
            }
        }
    }

    public class JsonFieldsCollector
    {
        private readonly Dictionary<string, JValue> fields;

        public JsonFieldsCollector(JToken token)
        {
            fields = new Dictionary<string, JValue>();
            CollectFields(token);
        }

        private void CollectFields(JToken jToken)
        {
            switch (jToken.Type)
            {
                case JTokenType.Object:
                    foreach (var child in jToken.Children<JProperty>())
                        CollectFields(child);
                    break;
                case JTokenType.Array:
                    foreach (var child in jToken.Children())
                        CollectFields(child);
                    break;
                case JTokenType.Property:
                    CollectFields(((JProperty)jToken).Value);
                    break;
                default:
                    fields.Add(jToken.Path, (JValue)jToken);
                    break;
            }
        }

        public IEnumerable<KeyValuePair<string, JValue>> GetAllFields() => fields;
    }
}
