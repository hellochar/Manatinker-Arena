using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateInstructions : MonoBehaviour {
    TMPro.TMP_Text text;
  // Start is called before the first frame update
  void Start() {
      text = GetComponent<TMPro.TMP_Text>();
  }

  // Update is called once per frame
  void Update() {
      text.text = EditModeInputController.instance.instructions;
  }
}
