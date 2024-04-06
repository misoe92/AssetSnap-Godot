shader_type spatial;

uniform sampler2D grass_texture : source_color;
uniform sampler2D grass_roughness_texture : source_color;
uniform sampler2D grass_normal_texture : source_color;

uniform sampler2D stone_texture : source_color;
uniform sampler2D stone_roughness_texture : source_color;
uniform sampler2D stone_normal_texture : source_color;

uniform sampler2D mountain_texture : source_color;
uniform sampler2D mountain_roughness_texture : source_color;
uniform sampler2D mountain_normal_texture : source_color;
uniform sampler2D noise_texture : source_color;

uniform sampler2D snow_texture : source_color;
uniform sampler2D snow_roughness_texture : source_color;
uniform sampler2D snow_normal_texture : source_color;

uniform sampler2D noise;
varying vec4 world_vertex;
varying float height;

void vertex() {
	// Sample noise texture
	UV = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xz * 0.1;		
	height = texture(noise, VERTEX.xz / 2.0 + 0.5).x * 55.0;
	
	//Pass UV coordinates to the fragment shader
	//from camera space to world space
	VERTEX.y = (MODEL_MATRIX * vec4(VERTEX, 1.0)).y;
	NORMAL = normalize((MODEL_MATRIX * vec4(NORMAL, 0.0)).xyz);
	world_vertex = (MODEL_MATRIX * vec4(VERTEX, 1.0));
	
    // Calculate distance from the center of the plane
    float distanceToCenter = length(world_vertex.xz);
    float centerRadius = 50.0; // Radius of the flat center area

    // Adjust the height based on distance to the center
    if (distanceToCenter <= centerRadius) {
        // Flat area within the center radius
        height = 0.0;
    }

  	VERTEX.y += height;
	height = VERTEX.y;
}

void fragment() {
   	// Get the height from the world-space Y coordinate
  	vec2 noisyUV = UV + texture(noise, UV * 10.0).rg * 0.01;

    // Sample textures using noisy UV coordinates
    vec4 grassColor = texture(grass_texture, noisyUV);
    vec4 mountainColor = texture(mountain_texture, noisyUV);
    // Use the height to blend textures
    vec4 stoneColor = texture(stone_texture, noisyUV);
    vec4 snowColor = texture(snow_texture, noisyUV);
	
	vec3 grassNormal = texture(grass_normal_texture, noisyUV).xyz * 2.0 - 1.0;
    vec3 stoneNormal = texture(stone_normal_texture, noisyUV).xyz * 2.0 - 1.0;
    vec3 mountainNormal = texture(mountain_normal_texture, noisyUV).xyz * 2.0 - 1.0;
    vec3 snowNormal = texture(snow_normal_texture, noisyUV).xyz * 2.0 - 1.0;
	
	float grassRoughness = texture(grass_roughness_texture, noisyUV).r;
    float stoneRoughness = texture(stone_roughness_texture, noisyUV).r;
    float mountainRoughness = texture(mountain_roughness_texture, noisyUV).r;
    float snowRoughness = texture(snow_roughness_texture, noisyUV).r;
	
	vec3 finalNormal = mix(grassNormal, stoneNormal, smoothstep(0.0, 0.1, height));
    finalNormal = mix(finalNormal, mountainNormal, smoothstep(0.1, .5, height));
    finalNormal = mix(finalNormal, snowNormal, smoothstep(12.0, 25.0, height));

	float finalRoughness = mix(grassRoughness, stoneRoughness, smoothstep(0.0, 0.1, height));
    finalRoughness = mix(finalRoughness, mountainRoughness, smoothstep(0.1, .5, height));
    finalRoughness = mix(finalRoughness, snowRoughness, smoothstep(12.0, 25.0, height));

    // Perform blending based on height
    vec4 finalColor = mix(grassColor, stoneColor, smoothstep(0.0,.1, height));
    finalColor = mix(finalColor, mountainColor, smoothstep(0.1,.5, height));
    finalColor = mix(finalColor, snowColor, smoothstep(12.0, 25.0, height));

   	// Output the final color and roughness
    ALBEDO = finalColor.rgb;
    ROUGHNESS = finalRoughness;
}