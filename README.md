# INFR4320U-Final-Project
conda activate UnityML
mlagents-learn config/ppo/FlappyBird.yaml --run-id=FlappyBirdJump --torch-device cuda:0
tensorboard --logdir results