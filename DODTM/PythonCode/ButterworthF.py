# Фильтр Баттерворта — один из типов электронных фильтров. Фильтры этого класса отличаются от
# других методом проектирования.
import cv2
import numpy as np
import matplotlib.pyplot as plt
import sys


imagePath = sys.argv[1]
pathToSave = sys.argv[2]

# откройте изображение
f = cv2.imread(imagePath, 0)

# преобразуйте изображение в частоту. домен и сдвинутый
F = np.fft.fft2(f)
Fshift = np.fft.fftshift(F)

plt.imshow(np.log1p(np.abs(Fshift)), cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'1. ПреобразованиеИзображенияВЧастоту.png')

# Фильтр нижних частот Баттерворта
M, N = f.shape
H = np.zeros((M, N), dtype=np.float32)
D0 = 10  # снижение частоты
n = 10  # порядок
for u in range(M):
    for v in range(N):
        D = np.sqrt((u - M / 2) ** 2 + (v - N / 2) ** 2)
        H[u, v] = 1 / (1 + (D / D0) ** n)

plt.imshow(H, cmap='gray')
plt.axis('off')
plt.savefig(pathToSave + f'2. ФильтрНижнихЧастотБаттерворта.png')
# plt.show()

# фильтры изображений в частотной области
Gshift = Fshift * H
G = np.fft.ifftshift(Gshift)
g = np.abs(np.fft.ifft2(G))

plt.imshow(g, cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'3. ФильтрыИзображенийВЧастотнойОбласти.png')

# Фильтр высоких частот Баттерворта
HPF = np.zeros((M, N), dtype=np.float32)
D0 = 10
n = 1
for u in range(M):
    for v in range(N):
        D = np.sqrt((u - M / 2) ** 2 + (v - N / 2) ** 2)

        if D == 0:
            D = 1

        HPF[u, v] = 1 / (1 + (D0 / D) ** n)

plt.imshow(HPF, cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'4. ФильтрВысокихЧастотБаттерворта.png')

# фильтры изображений в частотной области
Gshift = Fshift * HPF
G = np.fft.ifftshift(Gshift)
g = np.abs(np.fft.ifft2(G))

plt.imshow(g, cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'5. ФильтрыИзображенийВЧастотнойОбласти.png')
