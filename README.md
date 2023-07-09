# INFR4320U-Final-Project

To train AI, install Python and Anaconda.

Steps:
- Install Python 3.9.13 via python.org website
  - Install on all users, in a non-user directory (ex: `program files`), add to PATH
  - Make sure `py` and `python` work in the console
- Install Anaconda3 2023.03 (later versions should work, but if not use this version)
  - Install on all users, in a non-user directory (ex: `program-files`), make sure no spaces in directory path
  - Add `INSTALL_DIRECTORY\Anaconda3` and `INSTALL_DIRECTORY\Anaconda3\Scripts` to System PATH (if Windows, type `path` in Windows search, select `Environment Variables`, under `System Variables` edit `Path`, select `New`, input directory path)
  - Restart cmd if it was running
  - run `conda init cmd.exe` in admin cmd
  - Close cmd
- Download `ML-Agents Release 20` (if you are not using this project where it's already included)
  - For sake of example, it will be extracted to `PROJECT_DIRECTORY\ml-agents-release_20`
- Create Anaconda environment
  - Open cmd in `PROJECT_DIRECTORY\ml-agents-release_20` (open `PROJECT_DIRECTORY\ml-agents-release_20` in windows explorer, type `cmd` in directory path area and press enter)
  - run `conda create -n UnityML python=3.9`
  - run `conda activate UnityML`
  - run `pip install torch -f https://download.pytorch.org/whl/torch_stable.html` (if need CUDA, may have to uninstall with `pip uninstall torch`, purge cache with `pip cache purge`, and install again)
  - run `pip install -e ./ml-agents-envs`
  - run `pip install -e ./ml-agents`
  - run `pip install Onyx`
  - Check if installed with `mlagents-learn --help`
  - If protobuf error, install correct version with `pip install protobuf==3.20.*`
  - Check if installed with `mlagents-learn --help`
- Install Unity Package
  - Open Unity and install package in `PROJECT_DIRECTORY\ml-agents-release_20\com.unity.ml-agents\package.json`
- Train AI:
  - If not already, open cmd in `PROJECT_DIRECTORY\ml-agents-release_20` and run `conda activate UnityML`
  - Run `mlagents-learn config/ppo/NAME_OF_CONFIG_FILE.yaml --run-id=NAME_OF_TRAINING_ID` (Add `--resume` for resuming training or `--force` for restarting from scratch. Add `--torch-device cuda` if you want to train with a Nvidia GPU)
  - Run `tensorboard --logdir results` to get webstats
