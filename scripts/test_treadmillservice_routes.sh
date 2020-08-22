curl http://localhost:8002/treadmill/

curl --request POST http://localhost:8002/treadmill -d "{`"""s`""": 1}"
curl --request POST http://localhost:8002/treadmill -d "1"
curl --request POST http://localhost:8002/treadmill?speed=1 -d """"""
curl --request POST http://localhost:8002/treadmill/1 -d """"""

# Test [FromRoute] (-d """""" sets empty Content-Length)
curl --request POST http://localhost:8002/treadmill/test/1 -d """"""

# Test [FromQuery] (-d """""" sets empty Content-Length)
curl --request POST http://localhost:8002/treadmill/test?a="2" -d """"""

# Test [FromBody]
curl --request POST http://localhost:8002/treadmill/test -d "3"

# Test [FromRoute], [FromQuery], [FromBody]
curl --request POST http://localhost:8002/treadmill/test/1?b="2" -d "3"

# Test deserialization of JSON body
curl --request POST http://localhost:8002/treadmill/test/ -d "{`"""Duration`""": 1.0}"

curl http://localhost:8002/treadmill/