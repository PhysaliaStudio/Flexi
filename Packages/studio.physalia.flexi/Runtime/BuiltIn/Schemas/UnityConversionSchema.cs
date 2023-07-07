using UnityEngine;

namespace Physalia.Flexi
{
    public class UnityConversionSchema : ConversionSchema
    {
        public override void Handle(IConversionHandler handler)
        {
            //handler.Handle<Vector2, Vector2>(value => value);
            handler.Handle<Vector2, Vector3>(value => value);
            handler.Handle<Vector2, Vector4>(value => value);
            handler.Handle<Vector2, Vector2Int>(value => new Vector2Int((int)value.x, (int)value.y));
            handler.Handle<Vector2, Vector3Int>(value => new Vector3Int((int)value.x, (int)value.y, 0));

            handler.Handle<Vector3, Vector2>(value => value);
            //handler.Handle<Vector3, Vector3>(value => value);
            handler.Handle<Vector3, Vector4>(value => value);
            handler.Handle<Vector3, Vector2Int>(value => new Vector2Int((int)value.x, (int)value.y));
            handler.Handle<Vector3, Vector3Int>(value => new Vector3Int((int)value.x, (int)value.y, (int)value.z));

            handler.Handle<Vector4, Vector2>(value => value);
            handler.Handle<Vector4, Vector3>(value => value);
            //handler.Handle<Vector4, Vector4>(value => value);

            handler.Handle<Vector2Int, Vector2>(value => value);
            handler.Handle<Vector2Int, Vector3>(value => new Vector3(value.x, value.y, 0f));
            //handler.Handle<Vector2Int, Vector2Int>(value => value);
            handler.Handle<Vector2Int, Vector3Int>(value => (Vector3Int)value);

            handler.Handle<Vector3Int, Vector2>(value => new Vector2(value.x, value.y));
            handler.Handle<Vector3Int, Vector3>(value => value);
            handler.Handle<Vector3Int, Vector2Int>(value => (Vector2Int)value);
            //handler.Handle<Vector3Int, Vector3Int>(value => value);

            handler.Handle<Vector2, string>(value => value.ToString());
            handler.Handle<Vector3, string>(value => value.ToString());
            handler.Handle<Vector4, string>(value => value.ToString());
            handler.Handle<Vector2Int, string>(value => value.ToString());
            handler.Handle<Vector3Int, string>(value => value.ToString());
        }
    }
}
