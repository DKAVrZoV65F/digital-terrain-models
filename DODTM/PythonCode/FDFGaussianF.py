# Фильтр частотной области, Гауссовский фильтр.
import cv2
import numpy as np
import matplotlib.pyplot as plt
import sys


imagePath = sys.argv[1]
pathToSave = sys.argv[2]

# откройте изображение f
f = cv2.imread(imagePath, 0)

plt.imshow(f, cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'1. ОткройтеИзображение.png')

# преобразуйте изображение в частотную область, f --> F
F = np.fft.fft2(f)
Fshift = np.fft.fftshift(F)

plt.imshow(np.log1p(np.abs(F)), cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'2. ПреобразуйтеИзображениеВЧастотнуюОбласть.png')

plt.imshow(np.log1p(np.abs(Fshift)), cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'3. ПреобразуйтеИзображениеВЧастотнуюОбласть.png')

# Создать фильтр Гаусса: Фильтр нижних частот
M, N = f.shape
H = np.zeros((M, N), dtype=np.float32)
D0 = 10
for u in range(M):
    for v in range(N):
        D = np.sqrt((u-M/2)**2 + (v-N/2)**2)
        H[u, v] = np.exp(-D**2/(2*D0*D0))

plt.imshow(H, cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'4. ФильтрНижнихЧастот.png')

# Фильтры изображений
Gshift = Fshift * H
G = np.fft.ifftshift(Gshift)
g = np.abs(np.fft.ifft2(G))

plt.imshow(g, cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'5. ФильтрИзображений.png')

plt.imshow(np.log1p(np.abs(Gshift)), cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'6. ФильтрИзображений.png')

plt.imshow(np.log1p(np.abs(G)), cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'7. ФильтрИзображений.png')

# Гауссовский: фильтр высоких частот
HPF = 1 - H

plt.imshow(HPF, cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'8. ФильтрВысокихЧастот.png')

# Фильтры изображений
Gshift = Fshift * HPF
G = np.fft.ifftshift(Gshift)
g = np.abs(np.fft.ifft2(G))

plt.imshow(g, cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'9. ФильтрИзображений.png')

plt.imshow(np.log1p(np.abs(Gshift)), cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'10. ФильтрИзображений.png')

plt.imshow(np.log1p(np.abs(G)), cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'11. ФильтрИзображений.png')
