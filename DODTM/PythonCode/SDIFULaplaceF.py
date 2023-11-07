# Фильтр изображений пространственной области с использованием фильтра Лапласа
import cv2
import numpy as np
import matplotlib.pyplot as plt
import sys


imagePath = sys.argv[1]
pathToSave = sys.argv[2]

img = cv2.imread(imagePath, 0)

plt.figure(dpi=150)
plt.imshow(img, cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'1. ОткройтеИНормализуйтеИзображение.png')

# ядро 1
kernel = np.array([[0, 1, 0],
                   [1, -4, 1],
                   [0, 1, 0]])

LaplacianImage = cv2.filter2D(src=img,
                              ddepth=-1,
                              kernel=kernel)

plt.figure(dpi=150)
plt.imshow(LaplacianImage, cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'2. ПервыйЭтапОбработки.png')

c = -1
g = img + c*LaplacianImage

plt.figure(dpi=150)
plt.imshow(g, cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'3. ВторойЭтапОбработки.png')

gClip = np.clip(g, 0, 255)
plt.figure(dpi=150)
plt.imshow(gClip, cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'4. ТретийЭтапОбработки.png')
