using System;
using System.Linq;
using Domain;
using Services;
using Extensions;

namespace Console
{
    class Program
    {
        static void Main()
        {
            var library = new LibraryService();
            var analytics = new AnalyticsService(library);

            library.OnNewReservation += r => System.Console.WriteLine($"[EVENT] Nowa rezerwacja: {r.Item.Title} dla {r.UserEmail} (Id: {r.Id})");
            library.OnReservationCancelled += r => System.Console.WriteLine($"[EVENT] Rezerwacja anulowana: {r.Item.Title} (Id: {r.Id})");
            
            var id1 = library.NextItemId();
            library.AddItem(new Book(id1, "W pustyni i w puszczy", "Henryk Sienkiewicz", "978-83-01-00001-1"));
            var id2 = library.NextItemId();
            library.AddItem(new EBook(id2, "C# in Depth", "Jon Skeet", "978-1-23456-789-0", "PDF"));
            library.RegisterUser("ala@kot.pl");

            while (true)
            {
                System.Console.WriteLine("\n=== System Biblioteczny ===");
                System.Console.WriteLine("1. Dodaj książkę");
                System.Console.WriteLine("2. Dodaj e-booka");
                System.Console.WriteLine("3. Zarejestruj użytkownika");
                System.Console.WriteLine("4. Pokaż dostępne pozycje");
                System.Console.WriteLine("5. Zarezerwuj pozycję");
                System.Console.WriteLine("6. Anuluj rezerwację");
                System.Console.WriteLine("7. Moje rezerwacje");
                System.Console.WriteLine("8. Statystyki");
                System.Console.WriteLine("0. Wyjście");
                System.Console.Write("> ");

                var choice = System.Console.ReadLine();
                try
                {
                    switch (choice)
                    {
                        case "1":
                            System.Console.Write("Tytuł: "); var title = System.Console.ReadLine();
                            System.Console.Write("Autor: "); var author = System.Console.ReadLine();
                            System.Console.Write("ISBN: "); var isbn = System.Console.ReadLine();
                            var newId = library.NextItemId();
                            library.AddItem(new Book(newId, title ?? "", author ?? "", isbn ?? ""));
                            System.Console.WriteLine("Dodano książkę.");
                            break;
                        case "2":
                            System.Console.Write("Tytuł: "); var t = System.Console.ReadLine();
                            System.Console.Write("Autor: "); var a = System.Console.ReadLine();
                            System.Console.Write("ISBN: "); var i = System.Console.ReadLine();
                            System.Console.Write("Format (PDF/EPUB): "); var f = System.Console.ReadLine();
                            var eid = library.NextItemId();
                            library.AddItem(new EBook(eid, t ?? "", a ?? "", i ?? "", f ?? "PDF"));
                            System.Console.WriteLine("Dodano e-booka.");
                            break;
                        case "3":
                            System.Console.Write("Email użytkownika: "); var email = System.Console.ReadLine();
                            library.RegisterUser(email ?? throw new ArgumentException("email required"));
                            System.Console.WriteLine("Zarejestrowano użytkownika.");
                            break;
                        case "4":
                            System.Console.Write("Filtr (tytuł/autor) - zostaw puste, by wszystkie: "); var phrase = System.Console.ReadLine();
                            var available = library.ListAvailableItems().ToList();
                            var filtered = available.FindByTitleOrAuthor(phrase ?? "");
                            foreach (var item in filtered)
                                item.DisplayInfo();
                            break;
                        case "5":
                            System.Console.Write("ID pozycji: ");
                            if (!int.TryParse(System.Console.ReadLine(), out int idToReserve)) { System.Console.WriteLine("Niepoprawne Id"); break; }
                            System.Console.Write("Email: "); var u = System.Console.ReadLine();
                            System.Console.Write("Ile dni (domyślnie 7): "); var daysStr = System.Console.ReadLine();
                            var days = 7;
                            if (!string.IsNullOrWhiteSpace(daysStr) && int.TryParse(daysStr, out var parsed)) days = parsed;
                            var res = library.CreateReservation(idToReserve, u ?? throw new ArgumentException("email"), DateTime.Now, DateTime.Now.AddDays(days));
                            System.Console.WriteLine($"Utworzono rezerwację Id {res.Id}");
                            break;
                        case "6":
                            System.Console.Write("Id rezerwacji do anulowania: ");
                            if (!int.TryParse(System.Console.ReadLine(), out int rid)) { System.Console.WriteLine("Niepoprawne Id"); break; }
                            library.CancelReservation(rid);
                            System.Console.WriteLine("Anulowano rezerwację (jeśli istniała).");
                            break;
                        case "7":
                            System.Console.Write("Email: "); var ue = System.Console.ReadLine();
                            var userRes = library.GetUserReservations(ue ?? "");
                            foreach (var r in userRes)
                            {
                                System.Console.WriteLine($"ResId:{r.Id} Item:{r.Item.Title} From:{r.From} To:{r.To} Active:{r.IsActive} Cancelled:{r.IsCancelled}");
                            }
                            break;
                        case "8":
                            System.Console.WriteLine($"Średni czas wypożyczenia (dni): {analytics.AverageLoanLengthDays():F2}");
                            System.Console.WriteLine($"Łączna liczba rezerwacji: {analytics.TotalLoans()}");
                            System.Console.WriteLine($"Najpopularniejszy tytuł: {analytics.MostPopularItemTitle()}");
                            System.Console.WriteLine($"FulfillmentRate: {analytics.FulfillmentRate():P2}");
                            break;
                        case "0":
                            return;
                        default:
                            System.Console.WriteLine("Nieznana opcja.");
                            break;
                    }
                }
                catch (ArgumentException ex)
                {
                    System.Console.WriteLine("[BŁĄD] " + ex.Message);
                }
                catch (ReservationConflictException ex)
                {
                    System.Console.WriteLine("[CONFLICT] " + ex.Message);
                }
                catch (InvalidOperationException ex)
                {
                    System.Console.WriteLine("[INVALID] " + ex.Message);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("[ERROR] " + ex);
                }
            }
        }
    }
}
