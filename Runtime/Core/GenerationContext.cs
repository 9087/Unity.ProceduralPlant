using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace ProceduralPlant.Core
{
    public class GenerationContext : IDisposable
    {
        private static readonly ObjectPool<GenerationContext> pool = new(() => new GenerationContext(null), OnGet);

        private static void OnGet(GenerationContext context)
        {
            context._onPointArrived = null;
            context.buffer = null;
            context.current = Point.origin;
            context.last = Line.none;
        }

        private System.Action<Point> _onPointArrived;
        public event System.Action<Point> onPointArrived
        {
            add
            {
                _onPointArrived += value;
                value?.Invoke(this.current);
            }
            remove => _onPointArrived -= value;
        }

        public MeshBuffer buffer { get; private set; }
        
        public Point current { get; private set; } = Point.origin;
        
        public Line last { get; private set; } = Line.none;

        public GenerationContext() : this(null)
        {
        }

        private GenerationContext(MeshBuffer meshBuffer)
        {
            this.buffer = meshBuffer ?? new();
        }

        public GenerationContext Clone()
        {
            var context = pool.Get();
            context._onPointArrived = _onPointArrived;
            context.buffer = buffer;
            context.current = current;
            context.last = last;
            return context;
        }

        public void Dispose()
        {
            pool.Release(this);
        }

        public void MoveForwardWithoutLine(float length)
        {
            current = this.current.MoveForward(length);
            last = Line.none;
            _onPointArrived?.Invoke(current);
        }

        public void MoveForwardWithLine(float length, Symbol symbol)
        {
            var oldLast = this.last;
            var oldCurrent = this.current;
            var newCurrent = this.current.MoveForward(length);
            this.current = newCurrent;
            this.last = new Line(oldLast != Line.none ? oldLast.end : oldCurrent, newCurrent, symbol.diameterRange);
            this._onPointArrived?.Invoke(this.current);
        }

        public void Rotate(Quaternion delta)
        {
            current = this.current.Rotate(delta);
        }
    }
}
