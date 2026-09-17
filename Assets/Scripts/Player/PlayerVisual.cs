using UnityEngine;

namespace RushLanes
{
    public class PlayerVisual : MonoBehaviour
    {
        Transform _body;
        Transform _shield;
        Vector3 _baseScale = new Vector3(0.7f, 1.1f, 0.7f);

        public void Build(CharacterDef character)
        {
            _body = CreatePart("Torso", Vector3.up * 0.55f, _baseScale, character.Body);
            CreatePart("Head", Vector3.up * 1.28f, new Vector3(0.42f, 0.42f, 0.42f), character.Accent);
            CreatePart("Pack", new Vector3(0f, 0.7f, -0.28f), new Vector3(0.35f, 0.45f, 0.2f), character.Accent);

            var bubble = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bubble.name = "Shield";
            bubble.transform.SetParent(transform, false);
            bubble.transform.localScale = Vector3.one * 2.1f;
            Object.Destroy(bubble.GetComponent<Collider>());
            var r = bubble.GetComponent<Renderer>();
            var m = MaterialFactory.ColorMat(new Color(0.4f, 0.9f, 1f, 0.35f), true);
            r.material = m;
            _shield = bubble.transform;
            _shield.gameObject.SetActive(false);
        }

        public void Tick(bool sliding, bool jumping, bool grounded, bool shield)
        {
            if (_body == null)
                return;

            float bob = grounded && !sliding ? 1f + Mathf.Sin(Time.time * 12f) * 0.04f : 1f;
            Vector3 scale = sliding ? new Vector3(0.95f, 0.45f, 1.1f) : jumping ? new Vector3(0.62f, 1.25f, 0.62f) : _baseScale * bob;
            _body.localScale = Vector3.Lerp(_body.localScale, scale, Time.deltaTime * 12f);
            _body.localPosition = sliding ? Vector3.up * 0.28f : Vector3.up * 0.55f;
            if (_shield != null)
                _shield.gameObject.SetActive(shield);
        }

        Transform CreatePart(string name, Vector3 pos, Vector3 scale, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(transform, false);
            go.transform.localPosition = pos;
            go.transform.localScale = scale;
            Object.Destroy(go.GetComponent<Collider>());
            go.GetComponent<Renderer>().material = MaterialFactory.ColorMat(color, true);
            return go.transform;
        }
    }
}
