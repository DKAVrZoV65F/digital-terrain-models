# SVD
import cv2
import numpy as np
import matplotlib.pyplot as plt
import sys


# imagePath = 'Images/csgxhjnsqh7j.jpg'
# pathToSave = 'C:\\Users\\10\\Documents\\МиВлгу\\PyExec\\ImagesComponents(csgxhjnsqh7j)\\'
# rangeToGen = 10
imagePath = sys.argv[1]
pathToSave = sys.argv[2]
rangeToGen = int(sys.argv[3])

img = cv2.imread(imagePath)
# Преобразование изображения в серую гамму для более быстрого
# вычисление.
gray_image = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
# Вычисление SVD
u, s, v = np.linalg.svd(gray_image, full_matrices=False)
# print(f'u.shape:{u.shape},s.shape:{s.shape},v.shape:{v.shape}')     # Вычисляем компоненты
# построение изображений с различным количеством компонентов

if rangeToGen > s.shape:
    rangeToGen = s.shape

comps = []
for i in range(0, s.shape[0], 50):
    comps.append(i)

for i in range(len(comps)):
    low_rank = u[:, :comps[i]] @ np.diag(s[:comps[i]]) @ v[:comps[i], :]
    if i == 0:
        plt.imshow(low_rank, cmap='gray'),
        plt.title(f'Actual Image with n_components = {comps[i]}')
        plt.savefig(pathToSave + f'ActualImageWithN_components={comps[i]}')
    else:
        plt.imshow(low_rank, cmap='gray'),
        plt.title(f'n_components = {comps[i]}')
        plt.savefig(pathToSave + f'ActualImageWithN_components={comps[i]}')
