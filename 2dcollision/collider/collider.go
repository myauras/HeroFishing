package collider

type Circle struct {
    X, Y, Radius float64
}
type Rectangle1 struct {
	X      float64
	Y      float64
	Width  float64
	Height float64
}
type Rectangle2 struct {
	P1, P2, P3, P4 Vector2
}

type Vector2 struct {
	X, Y float64
}