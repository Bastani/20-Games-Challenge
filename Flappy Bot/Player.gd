extends CharacterBody2D

var fallAcceleration = 150
var maxVelocity = 100
var jumpSpeed = -80
var targetVelocity = Vector2.ZERO

@onready var animatedSprite2D = $AnimatedSprite2D

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func _physics_process(delta):
	
	if Input.is_key_pressed(KEY_SPACE):
		targetVelocity.y = jumpSpeed
		
	targetVelocity.y += fallAcceleration * delta
	
	if targetVelocity.y > maxVelocity:
		targetVelocity.y = maxVelocity
	
	velocity = targetVelocity
		
	if targetVelocity.y < 0:
		animatedSprite2D.play()
	else:
		animatedSprite2D.stop()
	
	move_and_slide()
	pass
