import 'dart:convert';
import 'dart:io';

import 'package:eventsource/eventsource.dart';

import 'models/Item.dart';

main() async {
  HttpOverrides.global = MyHttpOverrides();

  connect(
      'https://localhost:5001/api/AuthUser/OnUpdate',
      'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6IjEiLCJzdWIiOiJBZG1pbiIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWVpZGVudGlmaWVyIjoiMSIsImp0aSI6IjVmNzllNDA2LTk3MzgtNGFmYi1hZWMyLTliNDUxZmMzYTU0ZSIsIm5iZiI6MTYxMTc5NTA5NSwiZXhwIjoxNjE0Mzg3MDk1LCJpc3MiOiJJc3N1ZXIiLCJhdWQiOiJBdWRpZW5jZSJ9.gU_NIZFBaPNtGhgq6aQ7QVxPsuAx0ktlVVxpO_1OCeY',
      '4');
}

connect(String url, String token, String data) {
  EventSource.connect(url, headers: {
    'authorization': 'Bearer $token',
    'data': data,
  }).then((value) {
    value.listen((Event event) {
      print('New event:');
      print('  id: ${event.id}');
      print('  event: ${event.event}');
      print('  data: ${event.data}');
    });
  });
}

class MyHttpOverrides extends HttpOverrides {
  @override
  HttpClient createHttpClient(SecurityContext context) {
    return super.createHttpClient(context)
      ..badCertificateCallback =
          (X509Certificate cert, String host, int port) => true;
  }
}
