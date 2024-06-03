using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Physalia.Flexi
{
    public class NodeConverter : JsonConverter<Node>
    {
        public override Node ReadJson(JsonReader reader, Type objectType, Node existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);
            Node node = CreateNodeInstance(jsonObject);

            // ID
            JToken idToken = jsonObject[TokenKeys.NODE_ID];
            if (idToken == null)
            {
                node.id = 0;
            }
            else
            {
                node.id = (int)idToken;
            }

            // Position
            JToken positionToken = jsonObject[TokenKeys.NODE_POSITION];
            if (positionToken == null)
            {
                node.position = new Vector2(0f, 0f);
            }
            else
            {
                node.position = positionToken.ToObject<Vector2>();
            }

            // Custom Fields
            if (node is SubgraphNode subgraphNode)
            {
                JToken token = jsonObject[nameof(SubgraphNode.key)];
                subgraphNode.key = token.ToObject<string>();
            }
            else
            {
                ReadVariables(serializer, jsonObject, node);
            }

            return node;
        }

        private static void ReadVariables(JsonSerializer serializer, JObject jsonObject, Node node)
        {
            FieldInfo[] fields = node.GetType().GetFieldsIncludeBasePrivate();
            for (var i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];

                if (field.IsStatic)
                {
                    continue;
                }

                Type fieldType = field.FieldType;

                if (fieldType.IsAbstract)
                {
                    continue;
                }

                if (fieldType.IsDefined(typeof(NonSerializedAttribute), true))
                {
                    continue;
                }

                if (fieldType.IsSubclassOf(typeof(Inport)))
                {
                    if (field.GetValue(node) is Inport inport)
                    {
                        JToken jsonToken = jsonObject[field.Name];
                        if (jsonToken != null)
                        {
                            inport.DefaultValue = jsonToken.ToObject(inport.ValueType, serializer);
                        }
                    }
                }

                if (fieldType.IsSubclassOf(typeof(Variable)))
                {
                    if (field.GetValue(node) is Variable variable)
                    {
                        JToken jsonToken = jsonObject[field.Name];
                        if (jsonToken != null)
                        {
                            variable.Value = jsonToken.ToObject(variable.ValueType, serializer);
                        }
                    }
                }
            }
        }

        private static Node CreateNodeInstance(JObject jsonObject)
        {
            JToken typeToken = jsonObject[TokenKeys.NODE_TYPE];
            if (typeToken == null)
            {
                Logger.Error($"[{nameof(NodeConverter)}] Deserialize failed: Missing the type field");
                return new MissingNode("");
            }

            string typeName = typeToken.ToString();
            Type type = ReflectionUtilities.GetTypeByName(typeName);
            if (type == null)
            {
                Logger.Error($"[{nameof(NodeConverter)}] Deserialize failed: Cannot find the type from all assemblies, typeName: {typeName}");
                return new MissingNode(typeName);
            }

            return NodeFactory.Create(type);
        }

        public override void WriteJson(JsonWriter writer, Node value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartObject();

            // ID
            writer.WritePropertyName(TokenKeys.NODE_ID);
            writer.WriteValue(value.id);

            // Position
            writer.WritePropertyName(TokenKeys.NODE_POSITION);
            writer.WriteStartObject();
            writer.WritePropertyName(nameof(value.position.x));
            writer.WriteValue(value.position.x);
            writer.WritePropertyName(nameof(value.position.y));
            writer.WriteValue(value.position.y);
            writer.WriteEndObject();

            // Node Type
            writer.WritePropertyName(TokenKeys.NODE_TYPE);
            Type nodeType = value.GetType();
            writer.WriteValue(nodeType.FullName);

            // Custom Fields
            if (value is SubgraphNode subgraphNode)
            {
                writer.WritePropertyName(nameof(SubgraphNode.key));
                serializer.Serialize(writer, subgraphNode.key);
            }
            else
            {
                WriteVariables(writer, value, serializer, nodeType);
            }

            writer.WriteEndObject();
        }

        private static void WriteVariables(JsonWriter writer, Node value, JsonSerializer serializer, Type nodeType)
        {
            FieldInfo[] fields = nodeType.GetFieldsIncludeBasePrivate();
            for (var i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];

                if (field.IsStatic)
                {
                    continue;
                }

                Type fieldType = field.FieldType;

                if (fieldType.IsAbstract)
                {
                    continue;
                }

                if (fieldType.IsDefined(typeof(NonSerializedAttribute), true))
                {
                    continue;
                }

                if (fieldType.IsSubclassOf(typeof(Inport)))
                {
                    if (field.GetValue(value) is Inport inport)
                    {
                        if (inport.IsDefaultValueSet())
                        {
                            writer.WritePropertyName(field.Name);
                            serializer.Serialize(writer, inport.DefaultValue);
                        }
                    }
                }

                if (fieldType.IsSubclassOf(typeof(Variable)))
                {
                    writer.WritePropertyName(field.Name);

                    // Get the variable. If the variable is not defined, create a new instance.
                    if (field.GetValue(value) is not Variable variable)
                    {
                        variable = Activator.CreateInstance(fieldType) as Variable;
                    }

                    serializer.Serialize(writer, variable.Value);
                }
            }
        }
    }
}
