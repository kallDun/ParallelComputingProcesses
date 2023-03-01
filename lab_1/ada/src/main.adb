with Ada.Text_Io, Ada.Integer_Text_IO;
use Ada.Text_IO, Ada.Integer_Text_IO;

procedure Main is

   can_stop : boolean := false;
   pragma Atomic(can_stop);

   task type break_thread;
   task body break_thread is
   begin
      delay 30.0;
      can_stop := true;
   end break_thread;


   task type main_thread is
      entry Finish(Sum : Out Long_Long_Integer);
   end main_thread;
   task body main_thread is
      Sum : Long_Long_Integer := 0;
      i : Integer := 1;
   begin
      for i in 1..1000 loop
         Sum := Sum + 1;
      end loop;

      accept Finish (Sum : out Long_Long_Integer) do
         Sum := main_thread.Sum;
      end Finish;
   end main_thread;


   Threads_Count : Integer;

begin
   Threads_Count := 3;
   declare
      A : Array(1..Threads_Count) of main_thread;
      S : Array(1..Threads_Count) of Long_Long_Integer;
      i : Integer := 1;
   begin
      for i in A'range loop
         A(i).Finish(S(i));
         Put_Line(S(i)'img);
      end loop;
   end;

end Main;
