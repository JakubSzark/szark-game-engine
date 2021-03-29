#version 420

out vec4 FragColor;
in vec2 texCoord;

uniform sampler2D tex;
uniform vec3 uCol;

struct Sphere {
	vec3 position;
	float radius;
};

struct Ray {
	vec3 origin;
	vec3 direction;
};

const float STEP = 0.1;
const float MAX_DIST = 10.0;

vec3 cameraPos = vec3(0.0, 0.0, -3.0);
vec3 cameraDir = vec3(0.0, 0.0, 1.0);
vec3 lightDir = normalize(vec3(1.0, -1.0, -1.0));

Sphere sphere = Sphere(vec3(0.0, 0.0, 0.0), 1.0);

bool Sphere_Intersect(Sphere sphere, vec3 point) {
	return distance(sphere.position, point) < sphere.radius;
}

vec3 Sphere_Normal(Sphere sphere, vec3 point) {
	return normalize(point - sphere.position);
}

vec3 rayCast(Ray ray) {
	float dist = STEP;
	while (dist < MAX_DIST) {
		vec3 point = ray.origin + (ray.direction * dist);
		if (Sphere_Intersect(sphere, point)) {
			vec3 normal = Sphere_Normal(sphere, point);
			float ndotl = dot(normal, lightDir);
			return vec3(ndotl, ndotl, ndotl);
		}

		dist += STEP;
	}

	return vec3(0.0, 0.0, 0.0);
}

vec3 perpendicular(vec3 vec)
{
	return abs(vec.z) < abs(vec.x) ? 
		vec3(vec.y, -vec.x, 0) : vec3(0, -vec.z, vec.y);
}

void main() {
	vec3 result = vec3(0.0, 0.0, 0.0);

	result.xy = (texCoord * 2.0) - 1;
	result.y *= -1;

	vec3 perp = perpendicular(cameraDir);
	vec3 up = perp * result.y;
	vec3 left = normalize(cross(cameraDir, perp)) * result.x;

	Ray ray = Ray(cameraPos, normalize(cameraDir + left + up));
	result = rayCast(ray);

	FragColor = vec4(result, 1.0);
}