# INFR4320U-Final-Project

Final assignment for AI course.

To train neural network install Python and ideally a virtual python environment manager like Anaconda. Python does have a built in way to use virtual environments but I haven't tested it.

Steps:
- Install Python 3.9.13 via python.org website
  - Install on all users, in a non-user directory (ex: `program files`), add to PATH
  - Make sure `py` and `python` work in the console
- Install Anaconda3 2023.03 (later versions should work, but if not use this version)
  - Install on all users, in a non-user directory, make sure no spaces in directory path
  - Add `INSTALL_DIRECTORY\Anaconda3` and `INSTALL_DIRECTORY\Anaconda3\Scripts` to System PATH (if Windows; type `path` in Windows search, select `Environment Variables`, under `System Variables` edit `Path`, select `New`, and input directory path)
  - Close console if it was running
  - If on Windows open admin cmd and run `conda init cmd.exe`
  - Close cmd
- Download `ML-Agents Release 20` (if you are not using this project where it's already included)
  - For sake of example, it will be extracted to `PROJECT_DIRECTORY\ml-agents-release_20`
- Create Anaconda environment
  - Open cmd in `PROJECT_DIRECTORY\ml-agents-release_20` (Quick way to open cmd with a set directory in Windows is to open `PROJECT_DIRECTORY\ml-agents-release_20` in file explorer, type `cmd` in directory path area, and press enter)
  - Run `conda create -n UnityML python=3.9`
  - Run `conda activate UnityML`
  - Run `pip install torch -f https://download.pytorch.org/whl/torch_stable.html` (if need CUDA, may have to uninstall with `pip uninstall torch`, purge cache with `pip cache purge`, and install again)
  - Run `pip install -e ./ml-agents-envs`
  - Run `pip install -e ./ml-agents`
  - Run `pip install Onyx`
  - Check if installed with `mlagents-learn --help`
  - If protobuf error, install correct version with `pip install protobuf==3.20.*`
  - Check if installed with `mlagents-learn --help`
- Install Unity Package
  - Open Unity and install package in `PROJECT_DIRECTORY\ml-agents-release_20\com.unity.ml-agents\package.json`
- Train network:
  - If cmd isnt already opened with an active virtual Python environment, open cmd in `PROJECT_DIRECTORY\ml-agents-release_20` and run `conda activate UnityML`
  - Run `mlagents-learn config/ppo/NAME_OF_CONFIG_FILE.yaml --run-id=NAME_OF_TRAINING_ID` (Add `--resume` for resuming training or `--force` for restarting from scratch. Add `--torch-device cuda` if you want to train with a Nvidia GPU)
  - Run `tensorboard --logdir results` to get webstats
