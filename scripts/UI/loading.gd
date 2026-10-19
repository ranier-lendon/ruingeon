extends Control

var next_scene_path = "res://scenes/dungeon.tscn" 

var progress = []
var load_status = 0
var can_continue = false

@onready var loading_bar = $LoadingBar
@onready var label = $Message

func _ready():
	# Itago muna natin ang "Press any key" text habang naglo-load pa
	label.visible = false 
	
	ResourceLoader.load_threaded_request(next_scene_path)

func _process(_delta):
	# Kung tapos na mag-load, wag na ulitin ang checking
	if can_continue:
		return
		
	load_status = ResourceLoader.load_threaded_get_status(next_scene_path, progress)
	
	if loading_bar != null:
		loading_bar.value = progress[0] * 100
	
	# Kapag kumpleto na ang pag-load (100%)
	if load_status == ResourceLoader.THREAD_LOAD_LOADED:
		can_continue = true # I-set sa true para makapag-click na ang player
		
		# Ipakita ang text at palitan ang mensahe
		label.text = "Press any key to continue"
		label.visible = true 
		loading_bar.visible = false
		
		# Pansinin na inalis na natin dito ang change_scene_to_packed

func _input(event):
	if can_continue:
		if event is InputEventKey or event is InputEventMouseButton:
			if event.is_pressed():
				var new_scene = ResourceLoader.load_threaded_get(next_scene_path)
				get_tree().change_scene_to_packed(new_scene)
