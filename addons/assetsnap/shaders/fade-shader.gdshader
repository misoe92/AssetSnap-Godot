shader_type spatial;

uniform float alpha : hint_range(0.0, 1.0) = 0.0; // Alpha value for transparency

void fragment() {
    ALPHA *= alpha; // Multiply the alpha channel of the color with the alpha value
}