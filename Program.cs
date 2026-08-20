using System;

namespace SistemaGestionTienda
{
    // Estructura para almacenar la información de un producto
    struct Producto
    {
        public int Codigo;
        public string Nombre;
        public string Categoria;
        public double Precio;
        public int Cantidad;
    }

    // Estructura para almacenar la información de una venta
    struct Venta
    {
        public string CodigoVenta;
        public string NombreProducto;
        public int CantidadVendida;
        public double PrecioUnitario;
        public double Subtotal;
        public double Descuento;
        public double Impuesto;
        public double Total;
        public DateTime Fecha;
    }

    class Program
    {
        // ---- Constantes del programa ----
        const int MAX_PRODUCTOS = 50;
        const double PORCENTAJE_IMPUESTO = 0.13; // 13% de impuesto
        const double PORCENTAJE_DESCUENTO = 0.10; // 10% de descuento
        const double MONTO_MINIMO_DESCUENTO = 20.0; // subtotal mínimo para aplicar descuento

        // ---- Datos globales del programa ----
        static Producto[] productos = new Producto[MAX_PRODUCTOS];
        static int cantidadProductos = 0;

        static Venta ultimaVenta;
        static bool existeVenta = false;

        static void Main(string[] args)
        {
            bool salir = false;

            while (!salir)
            {
                MostrarMenu();
                int opcion = LeerOpcionMenu();

                switch (opcion)
                {
                    case 1:
                        RegistrarProducto();
                        break;
                    case 2:
                        MostrarProductos();
                        break;
                    case 3:
                        RealizarVenta();
                        break;
                    case 4:
                        ConsultarUltimaVenta();
                        break;
                    case 5:
                        salir = true;
                        Console.WriteLine("\nGracias por usar el Sistema de Gestión de Tienda.");
                        break;
                    default:
                        Console.WriteLine("\nOpción inválida. Intente nuevamente.");
                        break;
                }

                if (!salir)
                {
                    Console.WriteLine("\nPresione ENTER para continuar...");
                    Console.ReadLine();
                }
            }
        }

        // Muestra el menú principal en la consola
        static void MostrarMenu()
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine(" SISTEMA DE GESTIÓN DE TIENDA");
            Console.WriteLine("========================================");
            Console.WriteLine("1. Registrar producto");
            Console.WriteLine("2. Mostrar productos");
            Console.WriteLine("3. Realizar venta");
            Console.WriteLine("4. Consultar última venta");
            Console.WriteLine("5. Salir");
            Console.WriteLine("========================================");
            Console.Write("Seleccione una opción: ");
        }

        // Lee y valida la opción elegida por el usuario usando TryParse
        static int LeerOpcionMenu()
        {
            string entrada = Console.ReadLine()!;
            int opcion;

            bool esValido = int.TryParse(entrada, out opcion);

            if (!esValido)
            {
                return -1; // valor fuera de rango -> cae en "default" del switch
            }

            return opcion;
        }

        // Solicita los datos de un producto y lo registra en el arreglo
        static void RegistrarProducto()
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine(" REGISTRAR PRODUCTO");
            Console.WriteLine("========================================");

            if (cantidadProductos >= MAX_PRODUCTOS)
            {
                Console.WriteLine("\nNo es posible registrar más productos. Límite alcanzado.");
                return;
            }

            Producto nuevoProducto = new Producto();
            nuevoProducto.Codigo = 101 + cantidadProductos;

            Console.Write("Nombre del producto: ");
            nuevoProducto.Nombre = Console.ReadLine()!;

            Console.Write("Categoría: ");
            nuevoProducto.Categoria = Console.ReadLine()!;

            // Validación del precio utilizando TryParse
            double precio = 0;
            bool precioValido = false;
            do
            {
                Console.Write("Precio: $");
                string entradaPrecio = Console.ReadLine()!;
                precioValido = double.TryParse(entradaPrecio, out precio);

                if (!precioValido || precio < 0)
                {
                    Console.WriteLine("Precio inválido. Ingrese un número válido mayor o igual a 0.");
                    precioValido = false;
                }
            } while (!precioValido);

            nuevoProducto.Precio = Math.Round(precio, 2);

            // Validación de la cantidad utilizando TryParse
            int cantidad = 0;
            bool cantidadValida = false;
            do
            {
                Console.Write("Cantidad disponible: ");
                string entradaCantidad = Console.ReadLine()!;
                cantidadValida = int.TryParse(entradaCantidad, out cantidad);

                if (!cantidadValida || cantidad < 0)
                {
                    Console.WriteLine("Cantidad inválida. Ingrese un número entero mayor o igual a 0.");
                    cantidadValida = false;
                }
            } while (!cantidadValida);

            nuevoProducto.Cantidad = cantidad;

            productos[cantidadProductos] = nuevoProducto;
            cantidadProductos++;

            Console.WriteLine("\nProducto registrado exitosamente. Código asignado: " + nuevoProducto.Codigo);
        }

        // Muestra en consola todos los productos registrados
        static void MostrarProductos()
        {
            Console.Clear();
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine(" PRODUCTOS DISPONIBLES");
            Console.WriteLine("--------------------------------------------");

            if (cantidadProductos == 0)
            {
                Console.WriteLine("\nNo hay productos registrados todavía.");
                return;
            }

            for (int i = 0; i < cantidadProductos; i++)
            {
                Console.WriteLine();
                Console.WriteLine("Código: " + productos[i].Codigo);
                Console.WriteLine("Producto: " + productos[i].Nombre);
                Console.WriteLine("Categoría: " + productos[i].Categoria);
                Console.WriteLine("Precio: $" + productos[i].Precio.ToString("0.00"));
                Console.WriteLine("Existencia: " + productos[i].Cantidad);
            }

            Console.WriteLine("\n--------------------------------------------");
        }

        // Gestiona el proceso completo de una venta
        static void RealizarVenta()
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine(" REALIZAR VENTA");
            Console.WriteLine("========================================");

            if (cantidadProductos == 0)
            {
                Console.WriteLine("\nNo hay productos registrados. Registre un producto primero.");
                return;
            }

            MostrarProductos();

            Console.Write("\nIngrese el código del producto a vender: ");
            string entradaCodigo = Console.ReadLine()!;
            int codigoBuscado;
            bool codigoValido = int.TryParse(entradaCodigo, out codigoBuscado);

            if (!codigoValido)
            {
                Console.WriteLine("Código inválido.");
                return;
            }

            // Buscar el producto por código
            int indiceProducto = -1;
            for (int i = 0; i < cantidadProductos; i++)
            {
                if (productos[i].Codigo == codigoBuscado)
                {
                    indiceProducto = i;
                    break;
                }
            }

            if (indiceProducto == -1)
            {
                Console.WriteLine("El producto no existe.");
                return;
            }

            // Validación de la cantidad a vender con TryParse
            Console.Write("Ingrese la cantidad a vender: ");
            string entradaCantidad = Console.ReadLine()!;
            int cantidadVendida;
            bool cantidadValida = int.TryParse(entradaCantidad, out cantidadVendida);

            if (!cantidadValida || cantidadVendida <= 0)
            {
                Console.WriteLine("Cantidad inválida.");
                return;
            }

            // Comprobación de inventario suficiente
            if (cantidadVendida > productos[indiceProducto].Cantidad)
            {
                Console.WriteLine("No hay suficiente inventario para esta venta.");
                Console.WriteLine("Existencia actual: " + productos[indiceProducto].Cantidad);
                return;
            }

            // Cálculos de la venta
            double precioUnitario = productos[indiceProducto].Precio;
            double subtotal = CalcularSubtotal(precioUnitario, cantidadVendida);
            double descuento = CalcularDescuento(subtotal);
            double impuesto = CalcularImpuesto(subtotal - descuento);
            double total = CalcularTotal(subtotal, descuento, impuesto);

            // Actualizar el inventario
            productos[indiceProducto].Cantidad -= cantidadVendida;

            // Generar código de venta con Random
            string codigoVenta = GenerarCodigoVenta();

            // Guardar la venta como "última venta"
            ultimaVenta.CodigoVenta = codigoVenta;
            ultimaVenta.NombreProducto = productos[indiceProducto].Nombre;
            ultimaVenta.CantidadVendida = cantidadVendida;
            ultimaVenta.PrecioUnitario = precioUnitario;
            ultimaVenta.Subtotal = subtotal;
            ultimaVenta.Descuento = descuento;
            ultimaVenta.Impuesto = impuesto;
            ultimaVenta.Total = total;
            ultimaVenta.Fecha = DateTime.Now;
            existeVenta = true;

            MostrarComprobante(ultimaVenta);
        }

        // Calcula el subtotal de la venta (precio unitario * cantidad)
        static double CalcularSubtotal(double precioUnitario, int cantidad)
        {
            double subtotal = precioUnitario * cantidad;
            return Math.Round(subtotal, 2);
        }

        // Calcula el descuento aplicable según el subtotal (10% si supera el mínimo)
        static double CalcularDescuento(double subtotal)
        {
            double descuento;

            if (subtotal > MONTO_MINIMO_DESCUENTO)
            {
                descuento = subtotal * PORCENTAJE_DESCUENTO;
            }
            else
            {
                descuento = 0;
            }

            return Math.Round(descuento, 2);
        }

        // Calcula el impuesto sobre el monto luego de aplicar el descuento
        static double CalcularImpuesto(double montoConDescuento)
        {
            double impuesto = montoConDescuento * PORCENTAJE_IMPUESTO;
            return Math.Round(impuesto, 2);
        }

        // Calcula el total final de la venta
        static double CalcularTotal(double subtotal, double descuento, double impuesto)
        {
            double total = subtotal - descuento + impuesto;
            return Math.Round(total, 2);
        }

        // Genera un código de venta aleatorio utilizando Random
        static string GenerarCodigoVenta()
        {
            Random generador = new Random();
            int numeroAleatorio = generador.Next(10000, 99999);
            string codigo = "VT-" + numeroAleatorio;
            return codigo;
        }

        // Muestra el comprobante de la venta realizada
        static void MostrarComprobante(Venta venta)
        {
            Console.WriteLine("\n========================================");
            Console.WriteLine(" COMPROBANTE");
            Console.WriteLine("========================================");
            Console.WriteLine("Producto: " + venta.NombreProducto);
            Console.WriteLine("Cantidad: " + venta.CantidadVendida);
            Console.WriteLine("Precio unitario: $" + venta.PrecioUnitario.ToString("0.00"));
            Console.WriteLine();
            Console.WriteLine("Subtotal: $" + venta.Subtotal.ToString("0.00"));
            Console.WriteLine("Descuento: $" + venta.Descuento.ToString("0.00"));
            Console.WriteLine("Impuesto: $" + venta.Impuesto.ToString("0.00"));
            Console.WriteLine("Total: $" + venta.Total.ToString("0.00"));
            Console.WriteLine();
            Console.WriteLine("Código de venta: " + venta.CodigoVenta);
            Console.WriteLine("Fecha: " + venta.Fecha.ToString("dd/MM/yyyy HH:mm"));
            Console.WriteLine("========================================");
        }

        // Muestra la información de la última venta realizada
        static void ConsultarUltimaVenta()
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine(" CONSULTAR ÚLTIMA VENTA");
            Console.WriteLine("========================================");

            if (!existeVenta)
            {
                Console.WriteLine("\nAún no se ha realizado ninguna venta.");
                return;
            }

            MostrarComprobante(ultimaVenta);
        }
    }
}