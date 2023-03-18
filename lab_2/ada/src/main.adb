with Ada.Numerics.discrete_Random;
with Ada.Text_IO; use Ada.Text_IO;

procedure Main is

   dim : constant Integer := 100000;
   thread_num : constant Integer := 2;
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
      rndIndex : Integer;
      rndValue : Integer;
   begin
      for i in 1..dim loop
         arr(i) := 0;
      end loop;

      rndIndex := generate_random_number(0, dim);
      rndValue := generate_random_number(-1000000, 0);
      arr(rndIndex) := rndValue;

      Put_Line("");
      Put("Random generated index is");
      Put(rndIndex'img);
      Put(", random generated value is ");
      Put(rndValue'img);
   end GenerateArray;


   task type Thread is
      entry Init(thread_index : in Integer);
   end Thread;


   protected ThreadManager is
      procedure AddDoneTask(MinIndex : in Integer; MinValue : in Integer; ThreadIndex : in Integer);
      entry GetMinIndexAndValue(MinIndex : out Integer; MinValue : out Integer);
   private
      min_Index : Integer;
      min_Value : Integer;
      flag : Boolean := true;
      tasks_count : Integer;
   end ThreadManager;

   protected body ThreadManager is
      procedure AddDoneTask(MinIndex : in Integer; MinValue : in Integer; ThreadIndex : in Integer) is
      begin
         Put_Line("");
         Put("Min element in thread");
         Put(ThreadIndex'img);
         Put(" with index");
         Put(MinIndex'img);
         Put(" is ");
         Put(MinValue'img);

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
      ThreadManager.AddDoneTask(min_index, min_value, thread_index);
   end Thread;


   threads : array(1..thread_num) of Thread;


   procedure PrintResults is
      minIndex : Integer;
      minValue : Integer;
   begin
      ThreadManager.GetMinIndexAndValue(minIndex, minValue);

      Put_Line("");
      Put("Min index is");
      Put(minIndex'img);
      Put(", min value is ");
      Put(minValue'img);
   end PrintResults;


begin
   Put("Threads count is");
   Put(thread_num'img);

   GenerateArray;
   for i in 1..thread_num loop
      threads(i).Init(i);
   end loop;
   PrintResults;
end Main;
