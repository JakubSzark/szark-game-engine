using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Szark.ECS
{
    /// <summary>
    /// Result of retrieving from a BytePool
    /// </summary>
    public struct PoolResult<T>
    {
        public T Value;
        public bool IsValid;
    }

    /// <summary>
    /// Holds a string of bytes as a pool.
    /// Can only increase in size but allows invalidation.
    /// </summary>
    public class BytePool
    {
        /// <summary>
        /// Amount of elements in the pool.
        /// </summary>
        public int Count
        {
            get
            {
                int count = 0;
                foreach (var valid in validElements)
                    if (valid) count++;
                return count;
            }
        }

        /// <summary>
        /// Amount of bytes taken from internal storage
        /// </summary>
        public int InternalSize => data?.Length ?? 0;

        /// <summary>
        /// The amount of elements that the internal 
        /// storage can have before needing to resize.
        /// </summary>
        public int Capacity
        {
            get => InternalSize / Alignment;
            set
            {
                Array.Resize(ref data, value * Alignment);
                if (value <= validElements.Count) return;
                for (int i = 0; i < value - validElements.Count; i++)
                    validElements.Add(false);
            }
        }

        /// <summary>
        /// Size of elements in the pool
        /// </summary>
        public int Alignment { get; private set; }

        // Rate at which array increases in size
        private int sizeIncrease = 8;
        // Whether or not an element is valid
        private List<bool> validElements;
        // Data object that holds contents of pool
        private byte[]? data = null;

        public BytePool(int alignment)
        {
            Alignment = alignment;
            validElements = new List<bool>(sizeIncrease);
            for (int i = 0; i < sizeIncrease; i++)
                validElements.Add(false);
            data = new byte[Alignment * sizeIncrease];
        }

        /// <summary>
        /// Add an item to the end of the pool
        /// </summary>
        public void Add<T>(T item, bool valid = true)
            where T : struct
        {
            // Initialize data if it hasn't
            if (data == null)
            {
                Alignment = Marshal.SizeOf<T>();
                validElements = new List<bool>(sizeIncrease);
                data = new byte[Alignment * sizeIncrease];
            }
            else
            {
                // Check that item is same size as already included items
                if (Marshal.SizeOf(item) != Alignment)
                    throw new ArgumentException("T is incorrect size");
            }

            Push(valid);
            Assign(item, Count, valid);
        }

        /// <summary>
        /// Increase the size of the pool
        /// </summary>
        public void Push(bool valid = false)
        {
            if (data == null)
                throw new ApplicationException("BytePool internal buffer is null!");

            validElements.Add(valid);

            if (Count == Capacity)
            {
                Array.Resize(ref data, data.Length +
                    (Alignment * sizeIncrease));
                sizeIncrease *= 2;
            }
        }

        /// <summary>
        /// Retrieves T value from the specified index as a
        /// PoolResult that also holds whether the value is valid.
        /// </summary>
        public PoolResult<T> Get<T>(int index) where T : struct
        {
            if (data == null)
                throw new ApplicationException("BytePool internal buffer is null!");

            if (index < validElements.Count)
            {
                unsafe
                {
                    // Convert byte[] to T
                    byte* buffer = stackalloc byte[Alignment];
                    for (int i = 0; i < Alignment; i++)
                        buffer[i] = data[(index * Alignment) + i];
                    IntPtr ptr = new IntPtr(buffer);

                    return new PoolResult<T>()
                    {
                        Value = Marshal.PtrToStructure<T>(ptr),
                        IsValid = validElements[index]
                    };
                }
            }

            return new PoolResult<T>();
        }

        /// <summary>
        /// Checks if element at index is valid
        /// </summary>
        public bool IsValidAt(int index) =>
            index < validElements.Count && validElements[index];

        /// <summary>
        /// Assigns an item at an index. Validates index
        /// </summary>
        public void Assign<T>(T item, int index, bool valid = true)
            where T : struct
        {
            if (data == null)
                throw new ApplicationException("BytePool internal buffer is null!");

            // Check that item is same size as already included items
            if (Marshal.SizeOf(item) != Alignment)
                throw new ArgumentException("T is incorrect size");

            // Convert the T into bytes
            unsafe
            {
                byte* buffer = stackalloc byte[Alignment];
                IntPtr ptr = new IntPtr(buffer);
                Marshal.StructureToPtr<T>(item, ptr, true);
                int position = index * Alignment;

                for (int i = 0; i < Alignment; i++)
                    data[position + i] = buffer[i];
            }

            validElements[index] = valid;
        }

        /// <summary>
        /// Invalidates an item at an index
        /// </summary>
        public void Invalidate(int index) =>
            validElements[index] = false;

        /// <summary>
        /// Strips internal data array to Length.
        /// </summary>
        public void TrimExcess()
        {
            Array.Resize(ref data, Count * Alignment);
            validElements.TrimExcess();
        }

        /// <summary>
        /// Returns a Span over internal pool data
        /// </summary>
        public unsafe Span<T> AsSpan<T>() where T : struct
        {
            fixed (byte* dataPtr = data)
            {
                return new Span<T>(dataPtr, Game.Get<Game>()
                    .EntityManager.entities.Count);
            }
        }
    }
}