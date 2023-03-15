with Ada.Numerics.discrete_Random;
with Ada.Text_IO; use Ada.Text_IO;

procedure Main is

   dim : constant Integer := 10000;
   thread_num : constant Integer := 8;
   arr : array(1..dim) of Integer;

   function generate_random_number ( from: in Integer; to: in Integer) return Integer is
       subtype Rand_Range is Integer range from .. to;
       package Rand_Int is new Ada.Numerics.Discrete_Random(Rand_Range);
       use Rand_Int;
       gen : Rand_Int.Generator;
       ret_val: Rand_Range;
   begin
      Rand_Int.Reset(gen);
      ret_val := Random(gen);
      return ret_val;
   end;

   procedure GenerateArray is
      --rndIndex : Integer;
      --rndValue : Integer;
   begin
      --rndIndex := generate_random_number(0, dim);
      --rndValue := generate_random_number(-1000000, 0);
      arr(120) := -12354;
      --for i in 1..dim loop
      --   arr(i) := i;
      --end loop;
   end GenerateArray;


   task type Thread is
      entry Init(thread_index : in Integer);
   end Thread;


   protected ThreadManager is
      procedure AddDoneTask(MinIndex : out Integer; MinValue : out Integer);
      entry GetMinIndexAndValue(MinIndex : out Integer; MinValue : out Integer);
   private
      min_Index : Integer;
      min_Value : Integer;
      flag : Boolean := true;
      tasks_count : Integer;
   end ThreadManager;

   protected body ThreadManager is
      procedure AddDoneTask(MinIndex : out Integer; MinValue : out Integer) is
      begin
         if (flag) then
            min_Value := MinValue;
            min_Index := MinIndex;
            flag := false;
         else
            if (MinValue < min_Value) then
               min_Value := MinValue;
               min_Index := MinIndex;
            end if;
         end if;
         tasks_count := tasks_count + 1;
      end AddDoneTask;

      entry GetMinIndexAndValue(MinIndex : out Integer; MinValue : out Integer) when tasks_count = thread_num is
      begin
         MinIndex := min_Index;
         MinValue := min_Value;
      end GetMinIndexAndValue;
   end ThreadManager;


   task body Thread is
      min_index : Integer;
      min_value : Integer;
      start_index, finish_index : Integer;
      thread_index : Integer;
   begin
      accept Init(thread_index : in Integer) do
         Thread.thread_index := thread_index;
      end Init;

      start_index := ((thread_index - 1) * dim / thread_num) + 1;
      finish_index := thread_index * dim / thread_num;
      min_index := start_index;
      min_value := arr(min_index);

      for i in start_index..finish_index loop
         if (arr(i) < min_value) then
            min_index := i;
            min_value := arr(i);
         end if;
      end loop;
      ThreadManager.AddDoneTask(min_index, min_value);
   end Thread;



   threads : array(1..thread_num) of Thread;
   minIndex : Integer;
   minValue : Integer;
begin
   GenerateArray;
   for i in 1..thread_num loop
      threads(i).Init(i);
   end loop;
   ThreadManager.GetMinIndexAndValue(minIndex, minValue);

   Put_Line("Min index is ");
   Put_Line(minIndex'img);
   Put_Line("Min value is ");
   Put_Line(minValue'img);

end Main;
