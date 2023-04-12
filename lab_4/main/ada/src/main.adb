with Ada.Text_IO; use Ada.Text_IO;
with GNAT.Semaphores; use GNAT.Semaphores;

procedure Main is
   task type Phylosopher is
      entry Start(Id : Integer);
   end Phylosopher;

   Forks : array (1..5) of Counting_Semaphore(1, Default_Ceiling);

   task body Phylosopher is
      Id : Integer;
      Id_Left_Fork, Id_Right_Fork : Integer;
   begin
      accept Start (Id : in Integer) do
         Phylosopher.Id := Id;
      end Start;
      Id_Left_Fork := Id;
      Id_Right_Fork := Id rem 5 + 1;

      for I in 1..10 loop
         Put_Line("Phylosopher " & Id'Img & " thinking " & I'Img & " time");

         if Id_Left_Fork < Id_Right_Fork then
            Forks(Id_Left_Fork).Seize;
            Put_Line("Phylosopher " & Id'Img & " took left fork");

            Forks(Id_Right_Fork).Seize;
            Put_Line("Phylosopher " & Id'Img & " took right fork");
         else
            Forks(Id_Right_Fork).Seize;
            Put_Line("Phylosopher " & Id'Img & " took right fork");

            Forks(Id_Left_Fork).Seize;
            Put_Line("Phylosopher " & Id'Img & " took left fork");
         end if;


         Put_Line("Phylosopher " & Id'Img & " eating" & I'Img & " time");


         if Id_Left_Fork < Id_Right_Fork then
            Forks(Id_Right_Fork).Release;
            Put_Line("Phylosopher " & Id'Img & " put right fork");

            Forks(Id_Left_Fork).Release;
            Put_Line("Phylosopher " & Id'Img & " put left fork");
         else
            Forks(Id_Left_Fork).Release;
            Put_Line("Phylosopher " & Id'Img & " put left fork");

            Forks(Id_Right_Fork).Release;
            Put_Line("Phylosopher " & Id'Img & " put right fork");
         end if;
      end loop;
   end Phylosopher;

   Phylosophers : array (1..5) of Phylosopher;
Begin
   for I in Phylosophers'Range loop
      Phylosophers(I).Start(I);
   end loop;
end Main;
