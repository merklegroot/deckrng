extends Button

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	randomize()
	# print("ready")
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	# print("process")
	pass


func _pressed():
	var random_number = randi() % 100 + 1
	print(random_number)
	
	$"../Label2".text = str(random_number)
	print("Hello world!")
	
	
