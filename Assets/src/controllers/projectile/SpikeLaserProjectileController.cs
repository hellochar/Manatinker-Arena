using UnityEngine;

public class SpikeLaserProjectileController : LaserProjectileController {
  public override void Start() {
    base.Start();
    var gradient = lineRenderer.colorGradient;
    var intensityScalar = projectile.damage / 10;
    var alphaScalar = Util.MapLinear(projectile.damage, 0, 10, 0, 1);
    var colorKeys = gradient.colorKeys;
    for (int i = 0; i < colorKeys.Length; i++) {
      var key = colorKeys[i];
      // key.color.a *= alphaScalar;
      key.color.r *= intensityScalar;
      key.color.g *= intensityScalar;
      key.color.b *= intensityScalar;
      colorKeys[i] = key;
    }

    var alphaKeys = gradient.alphaKeys;
    for (int i = 0; i < alphaKeys.Length; i++) {
      var key = alphaKeys[i];
      // key.color.a *= alphaScalar;
      key.alpha *= alphaScalar;
      alphaKeys[i] = key;
    }
    gradient.SetKeys(
      colorKeys,
      alphaKeys
    );
    lineRenderer.colorGradient = gradient;
  }
}