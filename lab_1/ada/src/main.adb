with Ada.Text_Io, Ada.Integer_Text_IO;
use Ada.Text_IO, Ada.Integer_Text_IO;

procedure Main is

   IsWorking : boolean := true;
   pragma Volatile(IsWorking);
   ThreadsCount : Integer := 16;
   someMagicNumber : Integer := 0;


   -- break thread
   task type break_thread;
   task body break_thread is
   begin
      delay 10.0;
      IsWorking := False;
   end break_thread;


   -- main thread
   task type main_thread is
      entry Start;
      entry Finish(Sum : Out Integer; Elements : out Integer);
   end main_thread;

   task body main_thread is
      Sum : Integer := 0;
      Elements : Integer := 0;
   begin
      accept Start;

      loop
         Sum := Sum + ThreadsCount;
         Elements := Elements + 1;
         someMagicNumber := someMagicNumber + 1;
         exit when not IsWorking;
      end loop;
      accept Finish (Sum : out Integer; Elements : out Integer) do
         Sum := main_thread.Sum;
         Elements := main_thread.Elements;
      end Finish;
   end main_thread;


   -- variables
   A : Array(1..ThreadsCount) of main_thread;
   S : Array(1..ThreadsCount) of Integer;
   E : Array(1..ThreadsCount) of Integer;
   Item : String(1..100);
   Last : Natural;

   break : break_thread;
   pragma Volatile(break);


-- main body
begin

   for i in A'Range loop
      A(i).Start;
   end loop;

   for i in A'range loop
      A(i).Finish(S(i), E(i));
   end loop;

   for i in S'range loop
      Put("Sum of Thread");
      Put(i'Img);
      Put(" =");
      Put(S(i)'img);
      Put(", Elements =");
      Put(E(i)'Img);
      Put_Line("");
   end loop;

   Last := 100;
   Get_Line(Item, Last);

end Main;
