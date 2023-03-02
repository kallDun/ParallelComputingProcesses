with Ada.Text_Io, Ada.Integer_Text_IO;
use Ada.Text_IO, Ada.Integer_Text_IO;

procedure Main is

   IsWorking : boolean := true;
   pragma Atomic(IsWorking);
   ThreadsCount : Integer := 8;


   task type break_thread is
      entry Start;
   end break_thread;
   task body break_thread is
   begin
      accept Start do
         delay 10.0;
         IsWorking := False;
      end Start;
   end break_thread;


   task type main_thread is
      entry Start(ThreadIndex : Integer);
      entry Finish(Sum : Out Integer);
   end main_thread;
   task body main_thread is
      Sum : Integer := 0;
   begin
      accept Start (ThreadIndex : in Integer) do
         Sum := ThreadIndex;
         loop
            Sum := Sum + ThreadsCount;
            exit when not IsWorking;
         end loop;
      end Start;

      accept Finish (Sum : out Integer) do
         Sum := main_thread.Sum;
      end Finish;
   end main_thread;


begin
   declare
      A : Array(1..ThreadsCount) of main_thread;
      S : Array(1..ThreadsCount) of Integer;
      break : break_thread;
      i : Integer := 1;
   begin
      for i in A'Range loop
         A(i).Start(i);
      end loop;

      break.Start;

      for i in A'range loop
         A(i).Finish(S(i));
         Put_Line(S(i)'img);
      end loop;
   end;

end Main;
