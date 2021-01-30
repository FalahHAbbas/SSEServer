class Items {
  List<Item> items = [];

  Items();

  Items.fromJsonList(List<dynamic> jsonList) {
    if (jsonList == null) return;

    for (final i in jsonList) {
      final item = Item().fromJson(i);
      items.add(item);
    }
  }
}

class Item {
  int id;
  int quantity;
  double price;
  String name;

  Item({
    this.id,
    this.quantity,
    this.price,
    this.name,
  });

  Item fromJson(Map<String, dynamic> json) {
    id = json['id'];
    quantity = json['quantity'];
    price = json['price'];
    name = json['name'];
    return this;
  }

  Map<String, dynamic> toJson() {
    final data = <String, dynamic>{};
    data['id'] = id;
    data['quantity'] = quantity;
    data['price'] = price;
    data['name'] = name;
    return data;
  }
}
