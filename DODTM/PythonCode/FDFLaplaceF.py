# Фильтр частотной области Фильтр Лапласа
import cv2
import numpy as np
import matplotlib.pyplot as plt
import sys


imagePath = sys.argv[1]
pathToSave = sys.argv[2]

# откройте и нормализуйте изображение
f = cv2.imread(imagePath, 0)
f = f / 255

plt.figure(dpi=150)
plt.imshow(f, cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'1. ОткройтеИНормализуйтеИзображение.png')

# преобразование в частотную область
F = np.fft.fftshift(np.fft.fft2(f))

plt.figure(dpi=150)
plt.imshow(np.log1p(np.abs(F)), cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'2. ПреобразованиеВЧастотнуюОбласть.png')

# Фильтр Лапласа
P, Q = F.shape
H = np.zeros((P, Q), dtype=np.float32)
for u in range(P):
    for v in range(Q):
        H[u, v] = -4 * np.pi * np.pi * ((u - P / 2) ** 2 + (v - Q / 2) ** 2)

plt.imshow(H, cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'3. ФильтрЛапласа.png')

# Изображение Лапласа
Lap = H * F
Lap = np.fft.ifftshift(Lap)
Lap = np.real(np.fft.ifft2(Lap))

# преобразовать значение изображения Лапласа в диапазон [-1,1]
OldRange = np.max(Lap) - np.min(Lap)
NewRange = 1 - -1
LapScaled = (((Lap - np.min(Lap)) * NewRange) / OldRange) + -1

plt.figure(dpi=150)
plt.imshow(LapScaled, cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'4. ПреобразоватьЗначениеИзображенияЛапласа.png')

# улучшение имиджа?
# image ehancement
c = -1
g = f + c * LapScaled
g = np.clip(g, 0, 1)

plt.figure(figsize=(5, 7), dpi=150)
plt.imshow(g, cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'5. ImageEhancement.png')
