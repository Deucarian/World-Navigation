# Package Validation

Validation project:

`C:/Repositories/Deucarian/WorldNavigation-TestProject`

Unity version:

`6000.3.5f1`

## Results

Import:

- command: `Unity.exe -batchmode -quit -projectPath C:/Repositories/Deucarian/WorldNavigation-TestProject -logFile C:/Repositories/Deucarian/WorldNavigation-TestProject-import-2.log`
- result: passed, return code 0, no compiler errors

EditMode:

- command: `Unity.exe -batchmode -projectPath C:/Repositories/Deucarian/WorldNavigation-TestProject -executeMethod BatchTestRunner.RunEditMode -batchTestResults C:/Repositories/Deucarian/WorldNavigation-TestProject-edit-2.txt -logFile C:/Repositories/Deucarian/WorldNavigation-TestProject-edit-2.log`
- result: `result=Passed; passCount=10; failCount=0; skipCount=0; duration=14,420`
- repeat: `result=Passed; passCount=10; failCount=0; skipCount=0; duration=15,236`

PlayMode:

- command: `Unity.exe -batchmode -projectPath C:/Repositories/Deucarian/WorldNavigation-TestProject -runTests -testPlatform PlayMode -testResults C:/Repositories/Deucarian/WorldNavigation-TestProject-play-results.xml -logFile C:/Repositories/Deucarian/WorldNavigation-TestProject-play.log`
- result: XML `test-run` passed, `total=1`, `passed=1`, `failed=0`, `duration=0,0513481`
- repeat: XML `test-run` passed, `total=1`, `passed=1`, `failed=0`, `duration=0,046362`

## Benchmark

Durable path:

`C:/Repositories/Deucarian/WorldNavigation-TestProject/Logs/world-navigation-benchmark-results.json`

- 1,000 agents for 300 ticks: 96.627 ms, 0 bytes allocated
- 5,000 agents for 300 ticks: 464.841 ms, 0 bytes allocated
- 10,000 agents for 300 ticks: 1061.221 ms, 0 bytes allocated

Object complexity: memory pose accessor, no GameObject prefab. These are Unity EditMode Mono measurements, not mobile, IL2CPP, Burst, ECS, NavMesh, or Transform-overhead claims.
