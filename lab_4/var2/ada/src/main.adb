with Ada.Text_IO; use Ada.Text_IO;
with GNAT.Semaphores; use GNAT.Semaphores;

procedure Main is
   task type Phylosopher is
      entry Start(Id : Integer);
   end Phylosopher;

   Forks : array (1..5) of Counting_Semaphore(1, Default_Ceiling);
   Forks_InHand : array (1..5) of Boolean;

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

         <<TRY_AGAIN>>
         Forks(Id_Left_Fork).Seize;
         Forks_InHand(Id_Left_Fork) := True;
         Put_Line("Phylosopher " & Id'Img & " took left fork");

         if Forks_InHand(Id_Right_Fork) then
            Forks_InHand(Id_Left_Fork) := False;
            Forks(Id_Left_Fork).Release;
            goto TRY_AGAIN;
         end if;

         Forks(Id_Right_Fork).Seize;
         Forks_InHand(Id_Right_Fork) := True;
         Put_Line("Phylosopher " & Id'Img & " took right fork");

         Put_Line("Phylosopher " & Id'Img & " eating" & I'Img & " time");

         Forks_InHand(Id_Right_Fork) := False;
         Forks(Id_Right_Fork).Release;
         Put_Line("Phylosopher " & Id'Img & " put right fork");
         Forks_InHand(Id_Left_Fork) := False;
         Forks(Id_Left_Fork).Release;
         Put_Line("Phylosopher " & Id'Img & " put left fork");
      end loop;
   end Phylosopher;

   Phylosophers : array (1..5) of Phylosopher;
Begin
   for I in Phylosophers'Range loop
      Phylosophers(I).Start(I);
   end loop;
end Main;
