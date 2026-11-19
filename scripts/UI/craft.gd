extends Control

signal item_selected(item_name: String)
signal close_requested

@onready var items_container: VBoxContainer = $PanelContainer/MarginContainer/VBoxContainer/ScrollContainer/VBoxContainer
@onready var filter_all: Button = $PanelContainer/MarginContainer/VBoxContainer/HBoxContainer/Filters/All
@onready var filter_sword: Button = $PanelContainer/MarginContainer/VBoxContainer/HBoxContainer/Filters/Sword
@onready var filter_shield: Button = $PanelContainer/MarginContainer/VBoxContainer/HBoxContainer/Filters/Shield
@onready var filter_bow: Button = $PanelContainer/MarginContainer/VBoxContainer/HBoxContainer/Filters/Bow
@onready var close_button: Button = $PanelContainer/MarginContainer/VBoxContainer/HBoxContainer/CloseButton

var current_filter: String = "All"
var filter_buttons: Array[Button] = []

func _ready() -> void:
	filter_buttons = [filter_all, filter_sword, filter_shield, filter_bow]
	
	filter_all.pressed.connect(func(): set_filter("All"))
	filter_sword.pressed.connect(func(): set_filter("Sword"))
	filter_shield.pressed.connect(func(): set_filter("Shield"))
	filter_bow.pressed.connect(func(): set_filter("Bow"))
	
	close_button.pressed.connect(close)
	
	_connect_item_buttons()
	set_filter("All")

func close() -> void:
	close_requested.emit()
	hide()

func set_filter(filter_name: String) -> void:
	current_filter = filter_name
	
	# Button Highlights
	for btn in filter_buttons:
		if btn.name == filter_name:
			btn.modulate = Color(1.2, 1.1, 0.8, 1.0) # slight glow on active tab
		else:
			btn.modulate = Color(0.75, 0.75, 0.75, 1.0)
			
	# Filter items
	for child in items_container.get_children():
		if child is Button:
			child.visible = filter_name == "All" or child.is_in_group(filter_name)

func _connect_item_buttons() -> void:
	for child in items_container.get_children():
		if child is Button:
			child.pressed.connect(func(): _on_item_pressed(child.name))

func _on_item_pressed(item_name: String) -> void:
	item_selected.emit(item_name)
