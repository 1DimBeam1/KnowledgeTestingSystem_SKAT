-- =============================================
-- KnowledgeTestingSystem — Seed Data (безопасный для повторного запуска)
-- =============================================

-- 1. Группы
INSERT INTO groups (group_id, name) VALUES
(0, 'Все группы'),
(1, 'ИТ-201'),
(2, 'ИТ-202'),
(3, 'ИТ-203')
ON CONFLICT (group_id) DO NOTHING;

-- Сброс AUTOINCREMENT для groups
SELECT setval('groups_group_id_seq', (SELECT MAX(group_id) FROM groups));

-- 2. Пользователи
INSERT INTO users (user_id, group_id, role, name, login, password, icon_path) VALUES
-- Преподаватель (группа 0 = "Все группы")
(1, 0, 'Преподаватель', 'Иванов Иван Иванович', 'admin', '12345', ''),
-- Обучающиеся
(2, 1, 'Обучающийся', 'Петров Алексей Сергеевич', 'petrov', '12345', ''),
(3, 1, 'Обучающийся', 'Сидоров Дмитрий Андреевич', 'sidorov', '12345', ''),
(4, 1, 'Обучающийся', 'Козлова Мария Игоревна', 'kozlova', '12345', ''),
(5, 2, 'Обучающийся', 'Новиков Сергей Павлович', 'novikov', '12345', ''),
(6, 2, 'Обучающийся', 'Морозова Анна Владимировна', 'morozova', '12345', ''),
(7, 2, 'Обучающийся', 'Волков Артём Дмитриевич', 'volkov', '12345', ''),
(8, 3, 'Обучающийся', 'Лебедева Елена Николаевна', 'lebedeva', '12345', ''),
(9, 3, 'Обучающийся', 'Семёнов Кирилл Олегович', 'semenov', '12345', ''),
(10, 3, 'Обучающийся', 'Кузнецова Дарья Максимовна', 'kuznetsova', '12345', '')
ON CONFLICT (user_id) DO NOTHING;

-- Сброс AUTOINCREMENT для users
SELECT setval('users_user_id_seq', (SELECT MAX(user_id) FROM users));

-- 3. Алгоритмы
INSERT INTO algorithms (algo_id, src_path, pic_path, algo_name) VALUES
(1, '/app/code_files', '', 'Сортировка пузырьком'),
(2, '/app/code_files', '', 'Поиск минимума в массиве'),
(3, '/app/code_files', '', 'Бинарный поиск')
ON CONFLICT (algo_id) DO NOTHING;

-- Сброс AUTOINCREMENT для algorithms
SELECT setval('algorithms_algo_id_seq', (SELECT MAX(algo_id) FROM algorithms));

-- 4. Шаги алгоритмов — Сортировка пузырьком (algo_id=1)
INSERT INTO algorithmsteps (algo_id, algo_step, description, difficult) VALUES
(1, 1, 'Инициализация: переменные n, массив arr', 0.3),
(1, 2, 'Начало внешнего цикла i от 0 до n-1', 0.5),
(1, 3, 'Начало внутреннего цикла j от 0 до n-i-2', 0.5),
(1, 4, 'Сравнение: arr[j] > arr[j+1]', 0.4),
(1, 5, 'Обмен: temp = arr[j]; arr[j] = arr[j+1]; arr[j+1] = temp', 0.6),
(1, 6, 'Конец внутреннего цикла', 0.3),
(1, 7, 'Конец внешнего цикла', 0.3),
(1, 8, 'Возврат отсортированного массива', 0.2)
ON CONFLICT (algo_id, algo_step) DO NOTHING;

-- 5. Шаги алгоритмов — Поиск минимума (algo_id=2)
INSERT INTO algorithmsteps (algo_id, algo_step, description, difficult) VALUES
(2, 1, 'Инициализация: min = arr[0], minIndex = 0', 0.3),
(2, 2, 'Начало цикла i от 1 до n-1', 0.4),
(2, 3, 'Сравнение: arr[i] < min', 0.4),
(2, 4, 'Обновление: min = arr[i], minIndex = i', 0.5),
(2, 5, 'Конец цикла', 0.2),
(2, 6, 'Возврат minIndex', 0.2)
ON CONFLICT (algo_id, algo_step) DO NOTHING;

-- 6. Шаги алгоритмов — Бинарный поиск (algo_id=3)
INSERT INTO algorithmsteps (algo_id, algo_step, description, difficult) VALUES
(3, 1, 'Инициализация: left = 0, right = n-1', 0.3),
(3, 2, 'Пока left <= right', 0.4),
(3, 3, 'mid = left + (right - left) / 2', 0.5),
(3, 4, 'Сравнение: arr[mid] == target', 0.4),
(3, 5, 'arr[mid] < target: left = mid + 1', 0.5),
(3, 6, 'arr[mid] > target: right = mid - 1', 0.5),
(3, 7, 'Возврат mid или -1', 0.3)
ON CONFLICT (algo_id, algo_step) DO NOTHING;

-- 7. Тесты
INSERT INTO tests (test_id, algo_id, description, test_name, difficult, solved_count, unsolved_count) VALUES
(1, 1, 'Отсортировать массив из 5 элементов', 'Сортировка пузырьком — Базовый', 0.5, 0, 0),
(2, 1, 'Отсортировать массив из 10 элементов', 'Сортировка пузырьком — Продвинутый', 0.7, 0, 0),
(3, 2, 'Найти индекс минимального элемента', 'Поиск минимума', 0.4, 0, 0),
(4, 3, 'Найти элемент в отсортированном массиве', 'Бинарный поиск', 0.6, 0, 0)
ON CONFLICT (test_id) DO NOTHING;

-- Сброс AUTOINCREMENT для tests
SELECT setval('tests_test_id_seq', (SELECT MAX(test_id) FROM tests));

-- 8. Входные данные тестов
INSERT INTO testinputdata (test_id, var_name, var_value, var_type, line_number) VALUES
(1, 'arr', '5, 3, 8, 1, 2', 'int[]', 1),
(1, 'n', '5', 'int', 1),
(2, 'arr', '10, 7, 3, 1, 9, 4, 8, 2, 6, 5', 'int[]', 1),
(2, 'n', '10', 'int', 1),
(3, 'arr', '42, 17, 88, 3, 56', 'int[]', 1),
(3, 'n', '5', 'int', 1),
(4, 'arr', '1, 3, 5, 7, 9, 11, 13, 15', 'int[]', 1),
(4, 'n', '8', 'int', 1),
(4, 'target', '9', 'int', 1)
ON CONFLICT (test_id, var_name) DO NOTHING;

-- 9. Отслеживаемые переменные — Алгоритм 1 (Сортировка пузырьком)
INSERT INTO trackedvariables (sequence, algo_id, algo_step, line_number, var_name, var_type) VALUES
(1, 1, 1, 1, 'arr', 'int[]'),
(1, 1, 1, 1, 'n', 'int'),
(2, 1, 2, 3, 'i', 'int'),
(3, 1, 3, 4, 'j', 'int'),
(4, 1, 4, 5, 'arr[j]', 'int'),
(4, 1, 4, 5, 'arr[j+1]', 'int'),
(5, 1, 5, 6, 'temp', 'int'),
(5, 1, 5, 6, 'arr[j]', 'int'),
(5, 1, 5, 6, 'arr[j+1]', 'int')
ON CONFLICT (sequence) DO NOTHING;

-- 10. Отслеживаемые переменные — Алгоритм 2 (Поиск минимума)
INSERT INTO trackedvariables (sequence, algo_id, algo_step, line_number, var_name, var_type) VALUES
(1, 2, 1, 1, 'arr', 'int[]'),
(1, 2, 1, 1, 'min', 'int'),
(1, 2, 1, 1, 'minIndex', 'int'),
(2, 2, 2, 2, 'i', 'int'),
(3, 2, 3, 3, 'arr[i]', 'int'),
(3, 2, 3, 3, 'min', 'int'),
(4, 2, 4, 4, 'min', 'int'),
(4, 2, 4, 4, 'minIndex', 'int')
ON CONFLICT (sequence) DO NOTHING;

-- 11. Отслеживаемые переменные — Алгоритм 3 (Бинарный поиск)
INSERT INTO trackedvariables (sequence, algo_id, algo_step, line_number, var_name, var_type) VALUES
(1, 3, 1, 1, 'arr', 'int[]'),
(1, 3, 1, 1, 'left', 'int'),
(1, 3, 1, 1, 'right', 'int'),
(2, 3, 2, 2, 'left', 'int'),
(2, 3, 2, 2, 'right', 'int'),
(3, 3, 3, 3, 'mid', 'int'),
(4, 3, 4, 4, 'arr[mid]', 'int'),
(4, 3, 4, 4, 'target', 'int')
ON CONFLICT (sequence) DO NOTHING;

-- Сброс AUTOINCREMENT для trackedvariables
SELECT setval('trackedvariables_sequence_seq', (SELECT MAX(sequence) FROM trackedvariables));