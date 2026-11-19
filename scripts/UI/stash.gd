extends Control

signal slot_selected(slot_index: int)
signal close_requested

@onready var panel_container: PanelContainer = $PanelContainer
@onready var grid_container: GridContainer = $PanelContainer/MarginContainer/VBoxContainer/HBoxContainer2/ScrollContainer/GridContainer
@onready var filter_all: Button = $PanelContainer/MarginContainer/VBoxContainer/HBoxContainer2/MarginContainer/Filters/All
@onready var filter_sword: Button = $PanelContainer/MarginContainer/VBoxContainer/HBoxContainer2/MarginContainer/Filters/Sword
@onready var filter_shield: Button = $PanelContainer/MarginContainer/VBoxContainer/HBoxContainer2/MarginContainer/Filters/Shield
@onready var filter_bow: Button = $PanelContainer/MarginContainer/VBoxContainer/HBoxContainer2/MarginContainer/Filters/Bow
@onready var filter_materials: Button = $PanelContainer/MarginContainer/VBoxContainer/HBoxContainer2/MarginContainer/Filters/Materials
@onready var close_button: Button = $PanelContainer/MarginContainer/VBoxContainer/HBoxContainer/CloseButton

var current_filter: String = "All"
var filter_buttons: Array[Button] = []

func _ready() -> void:
	filter_buttons = [filter_all, filter_sword, filter_shield, filter_bow, filter_materials]
	
	filter_all.pressed.connect(func(): set_filter("All"))
	filter_sword.pressed.connect(func(): set_filter("Sword"))
	filter_shield.pressed.connect(func(): set_filter("Shield"))
	filter_bow.pressed.connect(func(): set_filter("Bow"))
	filter_materials.pressed.connect(func(): set_filter("Materials"))
	
	close_button.pressed.connect(close)
	
	_connect_slot_inputs()
	set_filter("All")

func _unhandled_input(event: InputEvent) -> void:
	if visible and event.is_action_pressed("ui_cancel"):
		close()
		get_viewport().set_input_as_handled()

func close() -> void:
	close_requested.emit()
	hide()

func set_filter(filter_name: String) -> void:
	current_filter = filter_name
	
	for btn in filter_buttons:
		if btn.name == filter_name:
			btn.modulate = Color(1.2, 1.1, 0.8, 1.0)
		else:
			btn.modulate = Color(0.75, 0.75, 0.75, 1.0)

func _connect_slot_inputs() -> void:
	var idx := 0
	for child in grid_container.get_children():
		if child is Control:
			var slot_idx := idx
			child.gui_input.connect(func(event: InputEvent):
				if event is InputEventMouseButton and event.pressed and event.button_index == MOUSE_BUTTON_LEFT:
					slot_selected.emit(slot_idx)
			)
			idx += 1

