using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ordenamiento
{
    public class Form1 : Form
    {
        private ComboBox cboCantidad;
        private Button btnEjecutar;
        private Button btnInfo;
        private Button btnReiniciar;
        private TableLayoutPanel tableResultados;
        private Label lblTituloBubble;
        private Label lblTituloMerge;
        private Label lblTituloQuick;
        private Label lblBubble;
        private Label lblMerge;
        private Label lblQuick;

        private readonly Color _baseResultBackColor = Color.FromArgb(45, 45, 45);

        public Form1()
        {
            InitializeComponent();
            btnEjecutar.Click += BtnEjecutar_Click;
            btnInfo.Click += BtnInfo_Click;
            btnReiniciar.Click += BtnReiniciar_Click; 
        }

        private void InitializeComponent()
        {
            cboCantidad = new ComboBox();
            btnEjecutar = new Button();
            btnInfo = new Button();
            btnReiniciar = new Button(); 
            tableResultados = new TableLayoutPanel();
            lblTituloBubble = new Label();
            lblTituloMerge = new Label();
            lblTituloQuick = new Label();
            lblBubble = new Label();
            lblMerge = new Label();
            lblQuick = new Label();

            SuspendLayout();

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(760, 180);
            BackColor = Color.FromArgb(30, 30, 30);
            ForeColor = Color.White;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Prueba de Ordenamiento";

            //comboBox
            cboCantidad.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCantidad.Items.AddRange(new object[] { "500", "1000", "1500", "2000","3000", "5000", "10000", "20000", "10000000" });
            cboCantidad.Location = new Point(12, 12);
            cboCantidad.Width = 120;

            //el botón ejecutar
            btnEjecutar.Location = new Point(150, 10);
            btnEjecutar.Size = new Size(160, 32);
            btnEjecutar.Text = "Ejecutar prueba";

            //botón info
            btnInfo.Location = new Point(320, 10);
            btnInfo.Size = new Size(160, 32);
            btnInfo.Text = "Explicación métodos";

            //botón reiniciar
            btnReiniciar.Location = new Point(490, 10);
            btnReiniciar.Size = new Size(160, 32);
            btnReiniciar.Text = "Reiniciar";
            btnReiniciar.TabIndex = 3;

            //tabla de resultados
            tableResultados.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableResultados.ColumnCount = 3;
            tableResultados.RowCount = 2;
            tableResultados.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tableResultados.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tableResultados.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));
            tableResultados.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            tableResultados.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F));
            tableResultados.Location = new Point(12, 60);
            tableResultados.Size = new Size(736, 92);

            SetupTitulo(lblTituloBubble, "Burbuja");
            SetupTitulo(lblTituloMerge, "Merge Sort");
            SetupTitulo(lblTituloQuick, "Quick Sort");

            SetupResultado(lblBubble);
            SetupResultado(lblMerge);
            SetupResultado(lblQuick);

            tableResultados.Controls.Add(lblTituloBubble, 0, 0);
            tableResultados.Controls.Add(lblTituloMerge, 1, 0);
            tableResultados.Controls.Add(lblTituloQuick, 2, 0);
            tableResultados.Controls.Add(lblBubble, 0, 1);
            tableResultados.Controls.Add(lblMerge, 1, 1);
            tableResultados.Controls.Add(lblQuick, 2, 1);

            Controls.Add(tableResultados);
            Controls.Add(btnReiniciar); 
            Controls.Add(btnInfo);
            Controls.Add(btnEjecutar);
            Controls.Add(cboCantidad);

            cboCantidad.SelectedIndex = 1;
            ResetUI();

            ResumeLayout(false);
        }

        private static void SetupTitulo(Label lbl, string texto) //función para configurar los títulos
        {
            lbl.Text = texto;
            lbl.Dock = DockStyle.Fill;
            lbl.TextAlign = ContentAlignment.MiddleCenter;
            lbl.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lbl.ForeColor = Color.Gainsboro;
        }

        private void SetupResultado(Label lbl) //función para configurar las etiquetas de resultados
        {
            lbl.Text = "—";
            lbl.Dock = DockStyle.Fill;
            lbl.TextAlign = ContentAlignment.MiddleCenter;
            lbl.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            lbl.ForeColor = Color.White;
            lbl.BackColor = _baseResultBackColor;
            lbl.Margin = new Padding(4);
        }

        private async void BtnEjecutar_Click(object? sender, EventArgs e) //boton para ejecutar
        {
            if (cboCantidad.SelectedItem is null) //verifica que se haya seleccionado una cantidad
            {
                MessageBox.Show("Selecciona una cantidad.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnEjecutar.Enabled = false;
            int n = int.Parse(cboCantidad.SelectedItem.ToString()!);

            ResetRankingVisual();

            lblBubble.Text = lblMerge.Text = lblQuick.Text = "Preparando datos…";
            await Task.Delay(50);

            var origen = new int[n]; //arreglo original
            var rnd = new Random(); //generador de números aleatorios
            for (int i = 0; i < n; i++)
                origen[i] = rnd.Next(int.MinValue, int.MaxValue);

            int[] aBubble = (int[])origen.Clone(); //clona el arreglo original para cada algoritmo
            int[] aMerge = (int[])origen.Clone(); 
            int[] aQuick = (int[])origen.Clone();

            lblBubble.Text = "Ordenando…";
            lblMerge.Text = "Ordenando…";
            lblQuick.Text = "Ordenando…";

            var resultado = await Task.Run(() => //ejecuta los algoritmos en un hilo separado
            {
                var sw = Stopwatch.StartNew(); //cronómetro

                BubbleSort(aBubble); //ejecuta burbuja
                sw.Stop(); //detiene el cronómetro
                var tBubble = sw.Elapsed; //tiempo burbuja
                bool okBubble = IsSortedAscending(aBubble);

                sw.Restart();
                MergeSort(aMerge); //ejecuta merge sort
                sw.Stop(); //detiene el cronómetro
                var tMerge = sw.Elapsed; //tiempo merge
                bool okMerge = IsSortedAscending(aMerge); //verifica si está ordenado

                sw.Restart(); //ejecuta quick sort
                QuickSort(aQuick); //lo mismo de arriba pero con quick sort
                sw.Stop(); 
                var tQuick = sw.Elapsed;
                bool okQuick = IsSortedAscending(aQuick);

                return (tBubble, okBubble, tMerge, okMerge, tQuick, okQuick); //retorna los resultados
            });
            //se muestran los resultados
            lblBubble.Text = $"Burbuja: {resultado.tBubble.TotalMilliseconds:N3} ms | OK: {resultado.okBubble}"; 
            lblMerge.Text = $"Merge: {resultado.tMerge.TotalMilliseconds:N3} ms | OK: {resultado.okMerge}"; 
            lblQuick.Text = $"Quick: {resultado.tQuick.TotalMilliseconds:N3} ms | OK: {resultado.okQuick}"; 

            ApplyRanking(resultado.tBubble, resultado.tMerge, resultado.tQuick); //aplica el ranking visual

            btnEjecutar.Enabled = true;
        }

        private void BtnInfo_Click(object? sender, EventArgs e)
        {
            using var f = new InfoForm();
            f.ShowDialog(this);
        }

        private void BtnReiniciar_Click(object? sender, EventArgs e)
        {
            ResetUI();
        }

        //restablece la interfaz al estado inicial
        private void ResetUI()
        {
            btnEjecutar.Enabled = true;
            ResetRankingVisual();
            lblBubble.Text = "Listo para ejecutar";
            lblMerge.Text = "Listo para ejecutar";
            lblQuick.Text = "Listo para ejecutar";
            //mantener la selección actual; si se desea forzar: cboCantidad.SelectedIndex = 1;
        }

        private void ResetRankingVisual() //restablece los colores de las etiquetas de resultados
        {
            lblBubble.BackColor = _baseResultBackColor; lblBubble.ForeColor = Color.White;
            lblMerge.BackColor = _baseResultBackColor; lblMerge.ForeColor = Color.White;
            lblQuick.BackColor = _baseResultBackColor; lblQuick.ForeColor = Color.White;
        }

        private void ApplyRanking(TimeSpan tBubble, TimeSpan tMerge, TimeSpan tQuick) //aplica colores según el ranking de tiempos
        {
            var items = new[]
            {
                (Label: lblBubble, Tiempo: tBubble),
                (Label: lblMerge,  Tiempo: tMerge),
                (Label: lblQuick,  Tiempo: tQuick)
            }.OrderBy(x => x.Tiempo).ToArray();

            Color oro = Color.Gold;
            Color plata = Color.Silver;
            Color bronce = Color.FromArgb(205, 127, 50);

            items[0].Label.BackColor = oro; items[0].Label.ForeColor = Color.Black;
            items[1].Label.BackColor = plata; items[1].Label.ForeColor = Color.Black;
            items[2].Label.BackColor = bronce; items[2].Label.ForeColor = Color.White;
        }

        //algoritmos
        private static void BubbleSort(int[] a) //no pues lo que dice su nombre
        {
            int n = a.Length; //tamaño del arreglo
            bool swapped; //indica si se hizo un intercambio
            do //bucle principal
            {
                swapped = false; //si no hay intercambios
                for (int i = 1; i < n; i++) //recorre el arreglo
                {
                    if (a[i - 1] > a[i]) //compara pares vecinos
                    {
                        (a[i - 1], a[i]) = (a[i], a[i - 1]); //intercambia si están mal
                        swapped = true; //marca que hubo un intercambio
                    }
                }
                n--; //reduce el tamaño del arreglo a revisar
            } while (swapped); //repite hasta que no haya intercambios
        }

        private static void MergeSort(int[] a) //método para hacer cosas de métodos
        {
            if (a.Length <= 1) return; //arreglo ya ordenado
            int[] temp = new int[a.Length]; //arreglo temporal
            MergeSort(a, 0, a.Length - 1, temp); //llama al método recursivo
        }

        private static void MergeSort(int[] a, int left, int right, int[] temp) //el método recursivo que se ocupa arriba
        {
            if (left >= right) return; //caso base
            int mid = left + ((right - left) >> 1); //encuentra el punto medio
            MergeSort(a, left, mid, temp); //ordena la mitad izquierda
            MergeSort(a, mid + 1, right, temp); //ordena la mitad derecha
            if (a[mid] <= a[mid + 1]) return; //ya están ordenados
            Merge(a, left, mid, right, temp); //mezcla las dos mitades
        }

        private static void Merge(int[] a, int left, int mid, int right, int[] temp) //mezcla dos mitades ordenadas
        {
            int i = left, j = mid + 1, k = left; //índices para las dos mitades y el arreglo temporal
            while (i <= mid && j <= right) //mientras haya elementos en ambas mitades
            {
                if (a[i] <= a[j]) temp[k++] = a[i++]; //copia el menor al arreglo temporal
                else temp[k++] = a[j++]; //copia el menor al arreglo temporal
            }
            while (i <= mid) temp[k++] = a[i++]; //copia los elementos restantes de la primera mitad
            while (j <= right) temp[k++] = a[j++]; //copia los elementos restantes de la segunda mitad
            for (int t = left; t <= right; t++)//copia de vuelta al arreglo original
                a[t] = temp[t]; //copia de vuelta al arreglo original
        }

        private static void QuickSort(int[] a) //método para bombardear pica pica
        {
            if (a.Length <= 1) return; //arreglo ya ordenado
            QuickSort(a, 0, a.Length - 1); //llama al método recursivo
        }

        private static void QuickSort(int[] a, int low, int high) //hola, soy el método recursivo
        {
            while (low < high) //bucle principal
            {
                int p = Partition(a, low, high); //particiona el arreglo
                if (p - low < high - p) //ordena la parte más pequeña primero
                {
                    QuickSort(a, low, p - 1); //llama recursivamente
                    low = p + 1; //ajusta el límite inferior
                }
                else
                {
                    QuickSort(a, p + 1, high); //llama recursivamente
                    high = p - 1; //ajusta el límite superior
                }
            }
        }

        private static int Partition(int[] a, int low, int high) //partición con mediana de tres
        {
            int mid = low + ((high - low) >> 1); //encuentra el punto medio
            if (a[mid] < a[low]) (a[low], a[mid]) = (a[mid], a[low]); //ordena low, mid, high
            if (a[high] < a[low]) (a[low], a[high]) = (a[high], a[low]); //intercambia si es necesario
            if (a[mid] < a[high]) (a[mid], a[high]) = (a[high], a[mid]); //los mismo

            int pivot = a[high]; //elige el pivote, pivote es el mayor de los tres
            int i = low - 1; //índice del elemento más pequeño
            for (int j = low; j < high; j++) //recorre el arreglo
            {
                if (a[j] <= pivot) //compara con el pivote
                {
                    i++; //incrementa el índice del elemento más pequeño
                    (a[i], a[j]) = (a[j], a[i]); //intercambia
                }
            }
            (a[i + 1], a[high]) = (a[high], a[i + 1]); //coloca el pivote en su posición correcta
            return i + 1;
        }

        private static bool IsSortedAscending(int[] a) //verifica si el arreglo está ordenado de forma ascendente
        {
            for (int i = 1; i < a.Length; i++) 
                if (a[i] < a[i - 1]) return false; //si encuentra un par fuera de orden, retorna falso
            return true;
        }

        //ventana con las explicaciones de los métodos
        private sealed class InfoForm : Form
        {
            public InfoForm() 
            {
                Text = "Explicación de métodos";
                StartPosition = FormStartPosition.CenterParent;
                Size = new Size(560, 480);
                BackColor = Color.FromArgb(32, 32, 32);

                var txt = new RichTextBox
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    BackColor = Color.FromArgb(40, 40, 40),
                    ForeColor = Color.Gainsboro,
                    BorderStyle = BorderStyle.None,
                    Font = new Font("Segoe UI", 10F),
                    DetectUrls = false
                };
                txt.Text = GetInfoText();
                Controls.Add(txt);
            }

            private static string GetInfoText() => //texto de explicación
@"RESUMEN DE MÉTODOS

BURBUJA
- Compara pares vecinos y los intercambia si están mal.
- Repite hasta que ya no haya cambios.
- Muy simple, muy lento para muchos datos.

MERGE SORT
- Divide el arreglo en mitades y luego las une ordenadas.
- Siempre rápido (n log n).
- Necesita un arreglo temporal.

QUICK SORT
- Elige un pivote y separa menores y mayores.
- Repite en cada parte.
- Promedio muy rápido (n log n), peor caso n^2.
- Normalmente el más veloz si el pivote se elige bien.

ELECCIÓN RÁPIDA
- Para aprender: Burbuja (pocos elementos).
- Estabilidad y resultado seguro: Merge Sort.
- Velocidad promedio y poca memoria extra: Quick Sort.";
        }
    }
}