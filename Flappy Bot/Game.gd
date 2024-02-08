extends Node

@onready var sky = $TileMapSky
@onready var groundArea2D = $Area2D
@onready var ground = $Area2D/TileMapGround
@onready var player = $Player
@onready var pipesNode = $Pipes
@onready var ui = $UI

const Pipe = preload("res://Pipe.tscn")

var pipes = []

const PipeRange = 20
const Speed = 100
const PipeSpacing = 128

# Called when the node enters the scene tree for the first time.
func _ready():
	_pause_game()
	groundArea2D.body_entered.connect(_body_entered)
	for i in 3:
		var pipe = Pipe.instantiate()
		pipe.position.y = 144/2
		pipe.position.x = -32
		pipes.append(pipe)
		pipesNode.add_child(pipe)
	pass # Replace with function body.

func _reset_game():
	player.position = Vector2(32, 144/2)
	for i in pipes.size():
		var pipe = pipes[i]
		pipe.position.y = 144/2
		pipe.position.x = 256 + i * PipeSpacing
		pipe.body_entered.connect(_body_entered)
	set_process(true)
	player.set_physics_process(true)
	ui.hide()
	pass

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	var skyPos = sky.position
	var groundPos = ground.position
	
	if abs(skyPos.x) > 1 and (int(skyPos.x) % 96) == 0:
		skyPos = Vector2.ZERO
		
	if abs(groundPos.x) > 1 and (int(groundPos.x) % 96) == 0:
		groundPos = Vector2.ZERO
		
	skyPos.x -= 0.7 * delta * Speed
	groundPos.x -= 1.0 * delta * Speed
	
	sky.position = skyPos
	ground.position = groundPos
	
	for i in pipes.size():
		var pipe = pipes[i]
		if pipe.position.x < -10:
			pipe.position.x += 256 + PipeSpacing
			pipe.position.y = 144/2 + randi_range(-PipeRange, PipeRange) - 10
		pipe.position.x -= 1.0 * delta * Speed
	pass

func _body_entered(body):
	if body is CharacterBody2D:
		_pause_game()
	pass
	
func _pause_game():
	set_process(false)
	player.set_physics_process(false)
	ui.show()
	pass

func _on_button_button_up():
	_reset_game()
	pass # Replace with function body.
