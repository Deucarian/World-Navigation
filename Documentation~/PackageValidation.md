# Package Validation

Validation project:

`C:/Repositories/Deucarian/WorldNavigation-TestProject`

Unity version:

`6000.3.5f1`

## Results

## Phase 1M Compatibility Results

World Navigation compatibility was updated for World Spawning `0.2.0` generic request names:

- `WorldSpawnRequest`
- `WorldSpawnableId`
- `WorldSpawnChannelId`
- `WorldSpawnRequestContext`

Runtime dependencies remain:

- `com.deucarian.gameplay-foundation`
- `com.deucarian.world-spawning`

No Encounters dependency is present in runtime or test asmdefs.

Import:

- command: `Unity.exe -batchmode -quit -projectPath C:/Repositories/Deucarian/WorldNavigation-TestProject -logFile C:/Repositories/Deucarian/WorldNavigation-TestProject/Unity-Phase1M-Import.log`
- result: completed with no compiler or package-manager errors. Unity emitted licensing/thread shutdown warnings during batch exit.

EditMode:

- run 1: `10` total, `10` passed, `0` failed. Results: `TestResults-Phase1M-WorldNavigation-EditMode-1.xml`.
- run 2: `10` total, `10` passed, `0` failed. Results: `TestResults-Phase1M-WorldNavigation-EditMode-2.xml`.

PlayMode:

- run 1: `1` total, `1` passed, `0` failed. Results: `TestResults-Phase1M-WorldNavigation-PlayMode-1.xml`.
- run 2: `1` total, `1` passed, `0` failed. Results: `TestResults-Phase1M-WorldNavigation-PlayMode-2.xml`.

Benchmark:

- path: `C:/Repositories/Deucarian/WorldNavigation-TestProject/Logs/world-navigation-benchmark-results.json`
- 1,000 agents for 300 ticks: `94.082 ms`, `0` bytes allocated.
- 5,000 agents for 300 ticks: `475.690 ms`, `0` bytes allocated.
- 10,000 agents for 300 ticks: `1010.432 ms`, `0` bytes allocated.

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
